using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L1.Core.Domain.Transaction.Events;
using L1.Core.Exceptions;
using Xunit;

namespace Tests.Unit.L1.Core.Transaction;

public class PaymentTests {
  [Fact]
  public void Create_ValidParameters_InitializesPendingPayment() {
    var orderId = Guid.NewGuid();

    var payment = Payment.Create(orderId, 150m, PaymentMethod.Stripe);

    Assert.Equal(orderId, payment.OrderId);
    Assert.Equal(150m, payment.Amount);
    Assert.Equal(PaymentMethod.Stripe, payment.Method);
    Assert.Equal(PaymentStatus.Pending, payment.Status);
    Assert.Null(payment.PaymentUrl);
    Assert.Null(payment.TransactionId);
    Assert.Empty(payment.DomainEvents);
  }

  [Fact]
  public void SetPaymentUrl_UpdatesPaymentUrl() {
    var payment = Payment.Create(Guid.NewGuid(), 150m, PaymentMethod.Stripe);

    payment.SetPaymentUrl("https://pay.example.com");

    Assert.Equal("https://pay.example.com", payment.PaymentUrl);
  }

  [Fact]
  public void MarkAsCompleted_WhenPending_ChangesStatusAndRaisesCompletedEvent() {
    var payment = Payment.Create(Guid.NewGuid(), 150m, PaymentMethod.Paypal);

    payment.MarkAsCompleted("txn-001");

    Assert.Equal(PaymentStatus.Succeeded, payment.Status);
    Assert.Equal("txn-001", payment.TransactionId);

    var completedEvent = Assert.IsType<PaymentCompletedEvent>(Assert.Single(payment.DomainEvents));
    Assert.Equal(payment.Id, completedEvent.AggregateId);
    Assert.Equal(payment.OrderId, completedEvent.OrderId);
    Assert.Equal(150m, completedEvent.Amount);
    Assert.Equal(PaymentMethod.Paypal, completedEvent.Method);
    Assert.Equal("txn-001", completedEvent.TransactionId);
  }

  [Fact]
  public void MarkAsCompleted_WhenAlreadySucceeded_DoesNothing() {
    var payment = Payment.Create(Guid.NewGuid(), 150m, PaymentMethod.Paypal);
    payment.MarkAsCompleted("txn-001");
    payment.ClearEvents();

    payment.MarkAsCompleted("txn-002");

    Assert.Equal(PaymentStatus.Succeeded, payment.Status);
    Assert.Equal("txn-001", payment.TransactionId);
    Assert.Empty(payment.DomainEvents);
  }

  [Fact]
  public void MarkAsCompleted_WhenPaymentIsNotPending_ThrowsDomainException() {
    var payment = Payment.Create(Guid.NewGuid(), 150m, PaymentMethod.Paypal);
    payment.MarkAsFailed();

    var exception = Assert.Throws<DomainException>(() => payment.MarkAsCompleted("txn-001"));

    Assert.Equal("Chỉ có thể hoàn tất thanh toán khi thanh toán ở trạng thái chờ", exception.Message);
  }

  [Fact]
  public void MarkAsFailed_WhenPending_ChangesStatusToFailed() {
    var payment = Payment.Create(Guid.NewGuid(), 150m, PaymentMethod.Stripe);

    payment.MarkAsFailed();

    Assert.Equal(PaymentStatus.Failed, payment.Status);
  }

  [Fact]
  public void MarkAsFailed_WhenPaymentIsNotPending_ThrowsDomainException() {
    var payment = Payment.Create(Guid.NewGuid(), 150m, PaymentMethod.Stripe);
    payment.MarkAsCompleted("txn-001");

    var exception = Assert.Throws<DomainException>(() => payment.MarkAsFailed());

    Assert.Equal("Chỉ có thể hủy thanh toán khi đang ở trạng thái chờ", exception.Message);
  }

  [Fact]
  public void Refund_WhenSucceeded_ChangesStatusAndRaisesRefundedEvent() {
    var payment = Payment.Create(Guid.NewGuid(), 150m, PaymentMethod.Stripe);
    payment.MarkAsCompleted("txn-001");
    payment.ClearEvents();

    payment.Refund();

    Assert.Equal(PaymentStatus.Refunded, payment.Status);
    var refundedEvent = Assert.IsType<PaymentRefundedEvent>(Assert.Single(payment.DomainEvents));
    Assert.Equal(payment.Id, refundedEvent.AggregateId);
    Assert.Equal(payment.OrderId, refundedEvent.OrderId);
    Assert.Equal(payment.Amount, refundedEvent.Amount);
    Assert.Equal(payment.Method, refundedEvent.Method);
    Assert.Equal("txn-001", refundedEvent.TransactionId);
  }

  [Fact]
  public void Refund_WhenPaymentIsNotSucceeded_ThrowsDomainException() {
    var payment = Payment.Create(Guid.NewGuid(), 150m, PaymentMethod.Stripe);

    var exception = Assert.Throws<DomainException>(() => payment.Refund());

    Assert.Equal("Chỉ có thể hoàn tiền thanh toán với thanh toán đã hoàn tất", exception.Message);
  }
}
