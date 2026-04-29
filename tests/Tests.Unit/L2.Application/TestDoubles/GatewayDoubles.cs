using System.Text.Json;
using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.Ports.Gateway;

namespace Tests.Unit.L2.Application.TestDoubles;

public sealed class StubPaymentGateway : IPaymentGateway {
  public string PaymentUrlResult { get; set; } = string.Empty;
  public (bool isSucceed, string transactionId) ClientVerificationResult { get; set; }
  public (bool isSucceed, Guid paymentId, string transactionId) WebhookVerificationResult { get; set; }
  public bool RefundPaymentResult { get; set; }
  public ClientPayload ClientPayloadResult { get; set; } = new StubClientPayload();
  public WebhookPayload WebhookPayloadResult { get; set; } = new StubWebhookPayload();
  public Payment? LastCreatePaymentInput { get; private set; }
  public Order? LastCreateOrderInput { get; private set; }
  public Payment? LastRefundPaymentInput { get; private set; }
  public JsonElement? LastClientPayloadJson { get; private set; }
  public JsonElement? LastWebhookPayloadJson { get; private set; }

  public Task<string> CreatePaymentUrl(Payment payment, Order order, CancellationToken ct = default) {
    LastCreatePaymentInput = payment;
    LastCreateOrderInput = order;
    return Task.FromResult(PaymentUrlResult);
  }

  public Task<(bool isSucceed, string transactionId)> VerifyClientPayment(
    ClientPayload payload,
    CancellationToken ct = default
  ) {
    return Task.FromResult(ClientVerificationResult);
  }

  public Task<(bool isSucceed, Guid paymentId, string transactionId)> VerifyWebhookPayment(
    WebhookPayload payload,
    CancellationToken ct = default
  ) {
    return Task.FromResult(WebhookVerificationResult);
  }

  public Task<bool> RefundPayment(Payment payment, CancellationToken ct = default) {
    LastRefundPaymentInput = payment;
    return Task.FromResult(RefundPaymentResult);
  }

  public ClientPayload ToClientPayload(JsonElement payload) {
    LastClientPayloadJson = payload;
    return ClientPayloadResult;
  }

  public WebhookPayload ToWebhookPayload(JsonElement payload) {
    LastWebhookPayloadJson = payload;
    return WebhookPayloadResult;
  }

  public sealed record StubClientPayload : ClientPayload;
  public sealed record StubWebhookPayload : WebhookPayload;
}

public sealed class StubGatewayFactory : IGatewayFactory {
  public Dictionary<PaymentMethod, IPaymentGateway> Gateways { get; } = [];
  public PaymentMethod? LastRequestedMethod { get; private set; }

  public IPaymentGateway CreatePaymentGateway(PaymentMethod method) {
    LastRequestedMethod = method;
    return Gateways[method];
  }
}
