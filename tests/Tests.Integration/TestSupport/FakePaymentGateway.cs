using System.Text.Json;
using L1.Core.Domain.Transaction.Entities;
using L2.Application.Ports.Gateway;

namespace Tests.Integration.TestSupport;

internal sealed class FakePaymentGateway : IPaymentGateway {
  public Task<string> CreatePaymentUrl(Payment payment, Order order, CancellationToken ct = default) {
    return Task.FromResult("https://gateway.test/pay");
  }

  public Task<(bool isSucceed, string transactionId)> VerifyClientPayment(
    ClientPayload payload,
    CancellationToken ct = default
  ) {
    return Task.FromResult((true, "txn"));
  }

  public Task<(bool isSucceed, Guid paymentId, string transactionId)> VerifyWebhookPayment(
    WebhookPayload payload,
    CancellationToken ct = default
  ) {
    return Task.FromResult((true, Guid.NewGuid(), "txn"));
  }

  public Task<bool> RefundPayment(Payment payment, CancellationToken ct = default) {
    return Task.FromResult(true);
  }

  public ClientPayload ToClientPayload(JsonElement payload) {
    throw new NotSupportedException();
  }

  public WebhookPayload ToWebhookPayload(JsonElement payload) {
    throw new NotSupportedException();
  }
}
