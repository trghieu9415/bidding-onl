using L1.Core.Domain.Transaction.Entities;

namespace L2.Application.Ports.Gateway;

public interface IPaymentGateway {
  Task<string> CreatePaymentUrl(Payment payment, Order order, CancellationToken ct = default);
  Task<(bool isSucceed, string transactionId)> VerifyPayment(GatewayPayload payload, CancellationToken ct = default);
  Task<bool> RefundPayment(Payment payment, CancellationToken ct = default);
  public GatewayPayload ToGatewayPayload(object payload);
}

public abstract record GatewayPayload;
