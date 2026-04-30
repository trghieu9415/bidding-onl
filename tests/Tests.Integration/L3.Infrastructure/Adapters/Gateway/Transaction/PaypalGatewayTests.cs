using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L1.Core.Domain.Transaction.ValueObjects;
using L3.Infrastructure.Adapters.Gateway.Transaction;
using L3.Infrastructure.Options;
using Tests.Integration.TestSupport;
using Xunit;

namespace Tests.Integration.L3.Infrastructure.Adapters.Gateway.Transaction;

public class PaypalGatewayTests {
  [Fact]
  public async Task CreatePaymentUrl_returns_approve_link_and_uses_bearer_token() {
    var handler = new RecordingHttpMessageHandler();
    handler.Enqueue(HttpStatusCode.OK, """{ "access_token": "access-1" }""");
    handler.Enqueue(HttpStatusCode.OK, """
      {
        "links": [
          { "rel": "approve", "href": "https://paypal.test/approve" }
        ]
      }
      """);

    using var client = new HttpClient(handler);
    var gateway = CreateGateway(client);

    var url = await gateway.CreatePaymentUrl(CreatePayment(), CreateOrder());

    Assert.Equal("https://paypal.test/approve", url);
    Assert.Equal(new Uri("https://api-m.sandbox.paypal.com/v1/oauth2/token"), handler.Requests[0].RequestUri);
    Assert.Equal(AuthenticationHeaderValue.Parse("Bearer access-1"), handler.Requests[1].Headers.Authorization);
    Assert.Equal(new Uri("https://api-m.sandbox.paypal.com/v2/checkout/orders"), handler.Requests[1].RequestUri);
  }

  [Fact]
  public async Task VerifyClientPayment_returns_transaction_id_when_capture_succeeds() {
    var handler = new RecordingHttpMessageHandler();
    handler.Enqueue(HttpStatusCode.OK, """{ "access_token": "access-1" }""");
    handler.Enqueue(HttpStatusCode.OK, """
      {
        "status": "COMPLETED",
        "purchase_units": [
          {
            "payments": {
              "captures": [
                { "id": "capture-1" }
              ]
            }
          }
        ]
      }
      """);

    using var client = new HttpClient(handler);
    var gateway = CreateGateway(client);

    var result = await gateway.VerifyClientPayment(new PaypalClientPayload("token-1", "payer-1"));

    Assert.True(result.isSucceed);
    Assert.Equal("capture-1", result.transactionId);
    Assert.Equal(new Uri("https://api-m.sandbox.paypal.com/v2/checkout/orders/token-1/capture"), handler.Requests[1].RequestUri);
  }

  [Fact]
  public async Task VerifyWebhookPayment_validates_signature_then_extracts_capture_and_payment_id() {
    var paymentId = Guid.NewGuid();
    var rawBody = $$"""
      {
        "event_type": "PAYMENT.CAPTURE.COMPLETED",
        "resource": {
          "id": "capture-1",
          "custom_id": "{{paymentId}}"
        }
      }
      """;

    var handler = new RecordingHttpMessageHandler();
    handler.Enqueue(HttpStatusCode.OK, """{ "access_token": "access-1" }""");
    handler.Enqueue(HttpStatusCode.OK, """{ "verification_status": "SUCCESS" }""");

    using var client = new HttpClient(handler);
    var gateway = CreateGateway(client);

    var payload = new PaypalWebhookPayload(
      "algo",
      "https://cert.test",
      "txn-id",
      "sig",
      "2026-04-29T00:00:00Z",
      rawBody
    );

    var result = await gateway.VerifyWebhookPayment(payload);

    Assert.True(result.isSucceed);
    Assert.Equal(paymentId, result.paymentId);
    Assert.Equal("capture-1", result.transactionId);
    Assert.Equal(new Uri("https://api-m.sandbox.paypal.com/v1/notifications/verify-webhook-signature"), handler.Requests[1].RequestUri);
  }

  [Fact]
  public async Task RefundPayment_returns_true_when_paypal_marks_refund_completed() {
    var payment = CreatePayment();
    payment.MarkAsCompleted("capture-1");

    var handler = new RecordingHttpMessageHandler();
    handler.Enqueue(HttpStatusCode.OK, """{ "access_token": "access-1" }""");
    handler.Enqueue(HttpStatusCode.OK, """{ "status": "COMPLETED" }""");

    using var client = new HttpClient(handler);
    var gateway = CreateGateway(client);

    var result = await gateway.RefundPayment(payment);

    Assert.True(result);
    Assert.Equal(new Uri("https://api-m.sandbox.paypal.com/v2/payments/captures/capture-1/refund"), handler.Requests[1].RequestUri);
  }

  [Fact]
  public void ToClientPayload_and_ToWebhookPayload_map_expected_properties() {
    var gateway = CreateGateway(new HttpClient(new RecordingHttpMessageHandler()));
    using var clientDocument = JsonDocument.Parse("""{ "token": "token-1", "PayerID": "payer-1" }""");
    using var webhookDocument = JsonDocument.Parse("""
      {
        "auth_algo": "algo",
        "cert_url": "https://cert.test",
        "transmission_id": "txn-id",
        "transmission_sig": "sig",
        "transmission_time": "2026-04-29T00:00:00Z",
        "raw_body": "{}"
      }
      """);

    var clientPayload = Assert.IsType<PaypalClientPayload>(gateway.ToClientPayload(clientDocument.RootElement));
    var webhookPayload = Assert.IsType<PaypalWebhookPayload>(gateway.ToWebhookPayload(webhookDocument.RootElement));

    Assert.Equal("token-1", clientPayload.Token);
    Assert.Equal("payer-1", clientPayload.PayerId);
    Assert.Equal("algo", webhookPayload.AuthAlgo);
    Assert.Equal("https://cert.test", webhookPayload.CertUrl);
  }

  private static PaypalGateway CreateGateway(HttpClient client) {
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    return new PaypalGateway(new PaypalSettings {
      ClientId = "client-id",
      ClientSecret = "client-secret",
      Mode = "sandbox",
      Currency = "USD",
      WebhookId = "webhook-id",
      SuccessUrl = "https://frontend.test/success",
      CancelUrl = "https://frontend.test/cancel"
    }, new FakeHttpClientFactory(client));
  }

  private static Payment CreatePayment() {
    return Payment.Create(Guid.NewGuid(), 26220m, PaymentMethod.Paypal);
  }

  private static Order CreateOrder() {
    return Order.Create(
      Guid.NewGuid(),
      "Bidder",
      "bidder@example.com",
      Guid.NewGuid(),
      Guid.NewGuid(),
      "Vintage Camera",
      "https://cdn.test/item.jpg",
      new Address("Receiver", "0123", "123 Test St")
    );
  }
}
