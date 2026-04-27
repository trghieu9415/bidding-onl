using System.Text.Json;
using L1.Core.Domain.Transaction.Entities;
using L2.Application.Ports.Gateway;
using L3.Infrastructure.Options;
using Stripe;
using Stripe.Checkout;

namespace L3.Infrastructure.Adapters.Gateway.Transaction;

public class StripeGateway : IPaymentGateway {
  private readonly StripeSettings _settings;

  public StripeGateway(StripeSettings settings) {
    StripeConfiguration.ApiKey = settings.SecretKey;
    StripeConfiguration.MaxNetworkRetries = settings.Retry;
    _settings = settings;
  }

  public async Task<string> CreatePaymentUrl(Payment payment, Order order, CancellationToken ct = default) {
    var options = new SessionCreateOptions {
      PaymentMethodTypes = ["card"],
      LineItems = [
        new SessionLineItemOptions {
          PriceData = new SessionLineItemPriceDataOptions {
            UnitAmount = (long)payment.Amount,
            Currency = _settings.Currency,
            ProductData = new SessionLineItemPriceDataProductDataOptions {
              Name = $"Thanh toán sản phẩm {order.CatalogName}",
              Description = $"Order ID: {order.Id}",
              Images = [order.CatalogImage]
            }
          },
          Quantity = 1
        }
      ],
      Mode = "payment",
      SuccessUrl = $"{_settings.SuccessUrl}?payment_id={payment.Id}&session_id={{CHECKOUT_SESSION_ID}}",
      CancelUrl = _settings.CancelUrl
    };

    var service = new SessionService();
    var session = await service.CreateAsync(options, cancellationToken: ct);
    return session.Url;
  }

  public async Task<(bool isSucceed, string transactionId)> VerifyClientPayment(ClientPayload payload,
    CancellationToken ct = default) {
    if (payload is not StripeClientPayload stripePayload) {
      return (false, string.Empty);
    }

    try {
      var service = new SessionService();
      var session = await service.GetAsync(stripePayload.SessionId, cancellationToken: ct);

      if (session.PaymentStatus == "paid") {
        return (true, session.PaymentIntentId);
      }
    } catch {
      return (false, string.Empty);
    }

    return (false, string.Empty);
  }

  public Task<(bool isSucceed, string transactionId)> VerifyWebhookPayment(
    WebhookPayload payload,
    CancellationToken ct = default
  ) {
    if (payload is not StripeWebhookPayload stripePayload) {
      return Task.FromResult((false, string.Empty));
    }

    try {
      var stripeEvent = EventUtility.ConstructEvent(
        stripePayload.RawBody,
        stripePayload.StripeSignature,
        _settings.EndpointSecret
      );

      if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted) {
        var session = stripeEvent.Data.Object as Session;

        if (session?.PaymentStatus == "paid" && !string.IsNullOrEmpty(session.PaymentIntentId)) {
          return Task.FromResult((true, session.PaymentIntentId));
        }
      }

      return Task.FromResult((false, string.Empty));
    } catch (StripeException) {
      return Task.FromResult((false, string.Empty));
    } catch {
      return Task.FromResult((false, string.Empty));
    }
  }

  public async Task<bool> RefundPayment(Payment payment, CancellationToken ct = default) {
    var options = new RefundCreateOptions {
      PaymentIntent = payment.TransactionId
    };

    var service = new RefundService();
    try {
      await service.CreateAsync(options, cancellationToken: ct);
      return true;
    } catch {
      return false;
    }
  }

  public ClientPayload ToClientPayload(JsonElement data) {
    var props = data.ExtractProperties("session_id");
    return new StripeClientPayload(props["session_id"]);
  }

  public WebhookPayload ToWebhookPayload(JsonElement payload) {
    var props = payload.ExtractProperties("raw_body", "stripe_signature");
    return new StripeWebhookPayload(props["raw_body"], props["stripe_signature"]);
  }
}

public record StripeClientPayload(string SessionId) : ClientPayload;

public record StripeWebhookPayload(string RawBody, string StripeSignature) : WebhookPayload;
