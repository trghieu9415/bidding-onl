using System.Text.Json;
using L1.Core.Domain.Transaction.Entities;

namespace L2.Application.Ports.Gateway;

public interface IPaymentGateway {
  Task<string> CreatePaymentUrl(Payment payment, Order order, CancellationToken ct = default);

  Task<(bool isSucceed, string transactionId)> VerifyClientPayment(ClientPayload payload,
    CancellationToken ct = default);

  Task<(bool isSucceed, string transactionId)> VerifyWebhookPayment(WebhookPayload payload,
    CancellationToken ct = default);

  Task<bool> RefundPayment(Payment payment, CancellationToken ct = default);
  public ClientPayload ToClientPayload(JsonElement payload);
  public WebhookPayload ToWebhookPayload(JsonElement payload);
}

public abstract record ClientPayload;

public abstract record WebhookPayload;
