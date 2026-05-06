using System.Diagnostics.CodeAnalysis;
using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;

namespace Tests.Common.Builders;

[ExcludeFromCodeCoverage]
public class PaymentBuilder {
  private decimal _amount = 150m;
  private PaymentMethod _method = PaymentMethod.Stripe;
  private Guid _orderId = Guid.NewGuid();

  public PaymentBuilder WithOrderId(Guid orderId) {
    _orderId = orderId;
    return this;
  }

  public PaymentBuilder WithAmount(decimal amount) {
    _amount = amount;
    return this;
  }

  public PaymentBuilder WithMethod(PaymentMethod method) {
    _method = method;
    return this;
  }

  public Payment Build() {
    return Payment.Create(_orderId, _amount, _method);
  }
}
