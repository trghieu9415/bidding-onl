using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using L1.Core.Domain.Transaction.Entities;
using L2.Application.Ports.Gateway;
using L3.Infrastructure.Options;

namespace L3.Infrastructure.Adapters.Gateway.Transaction;

public class PaypalGateway : IPaymentGateway {
  private readonly HttpClient _client;
  private readonly PaypalSettings _options;

  public PaypalGateway(PaypalSettings options, IHttpClientFactory clientFactory) {
    _options = options;
    _client = clientFactory.CreateClient(nameof(PaypalGateway));

    var baseUrl =
      options.Mode.Equals("live", StringComparison.CurrentCultureIgnoreCase)
        ? "https://api-m.paypal.com"
        : "https://api-m.sandbox.paypal.com";

    _client.BaseAddress = new Uri(baseUrl);
  }

  public async Task<string> CreatePaymentUrl(Payment payment, Order order, CancellationToken ct = default) {
    var token = await GetAccessTokenAsync(ct);
    if (string.IsNullOrEmpty(token)) {
      return string.Empty;
    }

    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    var amount = (payment.Amount / _options.ExchangeRate).ToString("0.00", CultureInfo.InvariantCulture);

    var orderRequest = new {
      intent = "CAPTURE",
      purchase_units = new[] {
        new {
          reference_id = payment.Id.ToString(),
          amount = new {
            currency_code = _options.Currency,
            value = amount
          },
          description = $"Thanh toán sản phẩm {order.CatalogName}"
        }
      },
      application_context = new {
        return_url = $"{_options.SuccessUrl}?payment_id={payment.Id}",
        cancel_url = _options.CancelUrl,
        user_action = "PAY_NOW"
      }
    };

    var content = new StringContent(
      JsonSerializer.Serialize(orderRequest),
      Encoding.UTF8, "application/json"
    );
    var response = await _client.PostAsync("/v2/checkout/orders", content, ct);

    if (!response.IsSuccessStatusCode) {
      return string.Empty;
    }

    var json = JsonNode.Parse(await response.Content.ReadAsStringAsync(ct));
    var approveLink = json?["links"]?
      .AsArray().FirstOrDefault(x => x?["rel"]?.ToString() == "approve")?["href"]?.ToString();

    return approveLink ?? string.Empty;
  }

  public async Task<(bool isSucceed, string transactionId)> VerifyPayment(GatewayPayload payload,
    CancellationToken ct = default) {
    if (payload is not PaypalGatewayPayload paypalPayload || string.IsNullOrEmpty(paypalPayload.Token)) {
      return (false, string.Empty);
    }

    try {
      var token = await GetAccessTokenAsync(ct);
      if (string.IsNullOrEmpty(token)) {
        return (false, string.Empty);
      }

      _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
      var response = await _client.PostAsync(
        $"/v2/checkout/orders/{paypalPayload.Token}/capture",
        content, ct
      );

      if (!response.IsSuccessStatusCode) {
        return (false, string.Empty);
      }

      var jsonBody = await response.Content.ReadAsStringAsync(ct);
      var json = JsonNode.Parse(jsonBody);

      var status = json?["status"]?.ToString();
      var captureId = json?["purchase_units"]?[0]?["payments"]?["captures"]?[0]?["id"]?.ToString();

      if (status == "COMPLETED" && !string.IsNullOrEmpty(captureId)) {
        return (true, captureId);
      }

      return (false, string.Empty);
    } catch {
      return (false, string.Empty);
    }
  }

  public async Task<bool> RefundPayment(Payment payment, CancellationToken ct = default) {
    try {
      var token = await GetAccessTokenAsync(ct);
      if (string.IsNullOrEmpty(token)) {
        return false;
      }

      _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var refundRequest = new {
        note_to_payer = "Hoàn tiền vé phim"
      };

      var content = new StringContent(
        JsonSerializer.Serialize(refundRequest),
        Encoding.UTF8,
        "application/json"
      );

      var response = await _client.PostAsync(
        $"/v2/payments/captures/{payment.TransactionId}/refund",
        content,
        ct
      );

      if (!response.IsSuccessStatusCode) {
        return false;
      }

      var json = JsonNode.Parse(await response.Content.ReadAsStringAsync(ct));
      var status = json?["status"]?.ToString();

      return status == "COMPLETED";
    } catch {
      return false;
    }
  }

  public GatewayPayload ToGatewayPayload(JsonElement data) {
    var props = data.ExtractProperties("token", "PayerID");
    return new PaypalGatewayPayload(props["token"], props["PayerID"]);
  }

  // NOTE: ========== [Private Helper] ==========
  private async Task<string> GetAccessTokenAsync(CancellationToken ct) {
    var authBytes = Encoding.ASCII.GetBytes($"{_options.ClientId}:{_options.ClientSecret}");
    var request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");
    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
    request.Content =
      new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

    var response = await _client.SendAsync(request, ct);
    if (!response.IsSuccessStatusCode) {
      return string.Empty;
    }

    var json = JsonNode.Parse(await response.Content.ReadAsStringAsync(ct));
    return json?["access_token"]?.ToString() ?? string.Empty;
  }
}

public record PaypalGatewayPayload(string Token, string PayerId = "") : GatewayPayload;
