using L1.Core.Base.Entity;
using L1.Core.Domain.Transaction.Enums;
using L1.Core.Domain.Transaction.Events;
using L1.Core.Exceptions;

namespace L1.Core.Domain.Transaction.Entities;

public class Payment : AggregateRoot {
  private Payment() {}

  public Guid OrderId { get; private set; }
  public decimal Amount { get; private set; }
  public string? PaymentUrl { get; private set; }
  public string? TransactionId { get; private set; }
  public PaymentMethod Method { get; private set; }
  public PaymentStatus Status { get; private set; }

  public static Payment Create(Guid orderId, decimal amount, PaymentMethod method) {
    return new Payment {
      OrderId = orderId,
      Amount = amount,
      Method = method,
      Status = PaymentStatus.Pending
    };
  }

  public void SetPaymentUrl(string paymentUrl) {
    PaymentUrl = paymentUrl;
  }

  public void MarkAsCompleted(Guid userId, string transactionId) {
    if (Status == PaymentStatus.Succeeded) {
      return;
    }

    if (Status != PaymentStatus.Pending) {
      throw new DomainException("Chỉ có thể hoàn tất thanh toán khi thanh toán ở trạng thái chờ");
    }

    TransactionId = transactionId;
    Status = PaymentStatus.Succeeded;
    AddDomainEvent(new PaymentCompletedEvent(Id, OrderId, userId, Amount, Method, TransactionId));
  }

  public void MarkAsFailed() {
    if (Status != PaymentStatus.Pending) {
      throw new DomainException("Chỉ có thể hủy thanh toán khi đang ở trạng thái chờ");
    }

    Status = PaymentStatus.Failed;
  }

  public void Refund() {
    if (Status != PaymentStatus.Succeeded) {
      throw new DomainException("Chỉ có thể hoàn tiền thanh toán với thanh toán đã hoàn tất");
    }

    Status = PaymentStatus.Refunded;
    AddDomainEvent(new PaymentRefundedEvent(Id, OrderId, Amount, Method, TransactionId));
  }
}
