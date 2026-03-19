using L1.Core.Domain.Transaction.Entities;
using L2.Application.Ports.Gateway;
using L3.Infrastructure.Configs.Options;
using Stripe;
using Stripe.Checkout;

namespace L3.Infrastructure.Adapters.Gateway.Transaction;

public class StripeGateway : IPaymentGateway {
  private readonly StripeOptions _options;

  public StripeGateway(StripeOptions options) {
    StripeConfiguration.ApiKey = options.SecretKey;
    StripeConfiguration.MaxNetworkRetries = options.Retry;
    _options = options;
  }

  public async Task<string> CreatePaymentUrl(Payment payment, Order order, CancellationToken ct = default) {
    var options = new SessionCreateOptions {
      PaymentMethodTypes = ["card"],
      LineItems = [
        new SessionLineItemOptions {
          PriceData = new SessionLineItemPriceDataOptions {
            UnitAmount = (long)payment.Amount,
            Currency = _options.Currency,
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
      SuccessUrl = $"{_options.SuccessUrl}?payment_id={payment.Id}&session_id={{CHECKOUT_SESSION_ID}}",
      CancelUrl = _options.CancelUrl
    };

    var service = new SessionService();
    var session = await service.CreateAsync(options, cancellationToken: ct);
    return session.Url;
  }

  public async Task<(bool isSucceed, string transactionId)> VerifyPayment(GatewayPayload payload,
    CancellationToken ct = default) {
    if (payload is not StripeGatewayPayload stripePayload) {
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

  public GatewayPayload ToGatewayPayload(object data) {
    var props = data.ExtractProperties("session_id");
    return new StripeGatewayPayload(props["session_id"]);
  }
}

public record StripeGatewayPayload(string SessionId) : GatewayPayload;
