using FluentAssertions;
using L1.Core.Domain.Transaction.Enums;
using L1.Core.Domain.Transaction.Events;
using L1.Core.Exceptions;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.Entities;

public class PaymentTests {
  [Fact]
  public void Create_ValidParameters_InitializesPendingPayment() {
    // Arrange
    var orderId = Guid.NewGuid();
    var builder = new PaymentBuilder()
      .WithOrderId(orderId)
      .WithAmount(150m)
      .WithMethod(PaymentMethod.Stripe);

    // Act
    var payment = builder.Build();

    // Assert
    payment.OrderId.Should().Be(orderId);
    payment.Amount.Should().Be(150m);
    payment.Method.Should().Be(PaymentMethod.Stripe);
    payment.Status.Should().Be(PaymentStatus.Pending);
    payment.PaymentUrl.Should().BeNull();
    payment.TransactionId.Should().BeNull();
    payment.DomainEvents.Should().BeEmpty();
  }

  [Fact]
  public void SetPaymentUrl_UpdatesPaymentUrl() {
    // Arrange
    var payment = new PaymentBuilder().Build();

    // Act
    payment.SetPaymentUrl("https://pay.example.com");

    // Assert
    payment.PaymentUrl.Should().Be("https://pay.example.com");
  }

  [Fact]
  public void MarkAsCompleted_WhenPending_ChangesStatusAndRaisesCompletedEvent() {
    // Arrange
    var payment = new PaymentBuilder().WithAmount(150m).WithMethod(PaymentMethod.Paypal).Build();

    // Act
    payment.MarkAsCompleted("txn-001");

    // Assert
    payment.Status.Should().Be(PaymentStatus.Succeeded);
    payment.TransactionId.Should().Be("txn-001");

    var completedEvent = payment.DomainEvents.Should().ContainSingle().Subject.As<PaymentCompletedEvent>();
    completedEvent.AggregateId.Should().Be(payment.Id);
    completedEvent.OrderId.Should().Be(payment.OrderId);
    completedEvent.Amount.Should().Be(150m);
    completedEvent.Method.Should().Be(PaymentMethod.Paypal);
    completedEvent.TransactionId.Should().Be("txn-001");
  }

  [Fact]
  public void MarkAsCompleted_WhenAlreadySucceeded_DoesNothing() {
    // Arrange
    var payment = new PaymentBuilder().WithMethod(PaymentMethod.Paypal).Build();
    payment.MarkAsCompleted("txn-001");
    payment.ClearEvents();

    // Act
    payment.MarkAsCompleted("txn-002");

    // Assert
    payment.Status.Should().Be(PaymentStatus.Succeeded);
    payment.TransactionId.Should().Be("txn-001");
    payment.DomainEvents.Should().BeEmpty();
  }

  [Fact]
  public void MarkAsCompleted_WhenPaymentIsNotPending_ThrowsDomainException() {
    // Arrange
    var payment = new PaymentBuilder().WithMethod(PaymentMethod.Paypal).Build();
    payment.MarkAsFailed();

    // Act
    var act = () => payment.MarkAsCompleted("txn-001");

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Chỉ có thể hoàn tất thanh toán khi thanh toán ở trạng thái chờ");
  }

  [Fact]
  public void MarkAsFailed_WhenPending_ChangesStatusToFailed() {
    // Arrange
    var payment = new PaymentBuilder().WithMethod(PaymentMethod.Stripe).Build();

    // Act
    payment.MarkAsFailed();

    // Assert
    payment.Status.Should().Be(PaymentStatus.Failed);
  }

  [Fact]
  public void MarkAsFailed_WhenPaymentIsNotPending_ThrowsDomainException() {
    // Arrange
    var payment = new PaymentBuilder().WithMethod(PaymentMethod.Stripe).Build();
    payment.MarkAsCompleted("txn-001");

    // Act
    var act = () => payment.MarkAsFailed();

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Chỉ có thể hủy thanh toán khi đang ở trạng thái chờ");
  }

  [Fact]
  public void Refund_WhenSucceeded_ChangesStatusAndRaisesRefundedEvent() {
    // Arrange
    var payment = new PaymentBuilder().WithAmount(150m).WithMethod(PaymentMethod.Stripe).Build();
    payment.MarkAsCompleted("txn-001");
    payment.ClearEvents();

    // Act
    payment.Refund();

    // Assert
    payment.Status.Should().Be(PaymentStatus.Refunded);
    var refundedEvent = payment.DomainEvents.Should().ContainSingle().Subject.As<PaymentRefundedEvent>();
    refundedEvent.AggregateId.Should().Be(payment.Id);
    refundedEvent.OrderId.Should().Be(payment.OrderId);
    refundedEvent.Amount.Should().Be(payment.Amount);
    refundedEvent.Method.Should().Be(payment.Method);
    refundedEvent.TransactionId.Should().Be("txn-001");
  }

  [Fact]
  public void Refund_WhenPaymentIsNotSucceeded_ThrowsDomainException() {
    // Arrange
    var payment = new PaymentBuilder().WithMethod(PaymentMethod.Stripe).Build();

    // Act
    var act = () => payment.Refund();

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Chỉ có thể hoàn tiền thanh toán với thanh toán đã hoàn tất");
  }
}
