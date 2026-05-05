using FluentAssertions;
using L1.Core.Domain.Transaction.Enums;
using L1.Core.Domain.Transaction.Events;
using L1.Core.Domain.Transaction.ValueObjects;
using L1.Core.Exceptions;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.Entities;

public class OrderTests {
  private static readonly Address ShippingAddress = new("John Doe", "0123456789", "123 Auction Street");

  [Fact]
  public void Create_ValidParameters_InitializesPendingOrderAndRaisesCreatedEvent() {
    // Arrange
    var bidderId = Guid.NewGuid();
    var auctionId = Guid.NewGuid();
    var catalogId = Guid.NewGuid();
    var builder = new OrderBuilder()
      .WithBidderId(bidderId)
      .WithAuctionId(auctionId)
      .WithCatalogId(catalogId)
      .WithAddress(ShippingAddress);

    // Act
    var order = builder.Build();

    // Assert
    order.BidderId.Should().Be(bidderId);
    order.BidderName.Should().Be("John Doe");
    order.BidderEmail.Should().Be("john@example.com");
    order.AuctionId.Should().Be(auctionId);
    order.CatalogId.Should().Be(catalogId);
    order.CatalogName.Should().Be("Laptop");
    order.CatalogImage.Should().Be("main.png");
    order.Address.Should().Be(ShippingAddress);
    order.Status.Should().Be(OrderStatus.Pending);
    order.Price.Should().Be(0m);

    var createdEvent = order.DomainEvents.Should().ContainSingle().Subject.As<OrderCreatedEvent>();
    createdEvent.AggregateId.Should().Be(order.Id);
    createdEvent.BidderId.Should().Be(bidderId);
    createdEvent.AuctionId.Should().Be(auctionId);
  }

  [Fact]
  public void MarkAsPaid_WhenPending_ChangesStatusAndRaisesCompletedEvent() {
    // Arrange
    var order = new OrderBuilder().Build();
    order.ClearEvents();

    // Act
    order.MarkAsPaid("billing@example.com");

    // Assert
    order.Status.Should().Be(OrderStatus.Confirmed);
    var completedEvent = order.DomainEvents.Should().ContainSingle().Subject.As<OrderCompletedEvent>();
    completedEvent.AggregateId.Should().Be(order.Id);
    completedEvent.BidderId.Should().Be(order.BidderId);
    completedEvent.AuctionId.Should().Be(order.AuctionId);
    completedEvent.BidderName.Should().Be(order.BidderName);
    completedEvent.BidderEmail.Should().Be("billing@example.com");
  }

  [Fact]
  public void MarkAsPaid_WhenNotPending_ThrowsDomainException() {
    // Arrange
    var order = new OrderBuilder().Build();
    order.Cancel();

    // Act
    var act = () => order.MarkAsPaid("billing@example.com");

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Chỉ có thể thanh toán đơn khi đơn ở trạng thái Chờ");
  }

  [Fact]
  public void Cancel_WhenPending_ChangesStatusAndRaisesCanceledEvent() {
    // Arrange
    var order = new OrderBuilder().Build();
    order.ClearEvents();

    // Act
    order.Cancel();

    // Assert
    order.Status.Should().Be(OrderStatus.Canceled);
    var canceledEvent = order.DomainEvents.Should().ContainSingle().Subject.As<OrderCanceledEvent>();
    canceledEvent.AggregateId.Should().Be(order.Id);
    canceledEvent.CustomerId.Should().Be(order.BidderId);
    canceledEvent.AuctionId.Should().Be(order.AuctionId);
  }

  [Fact]
  public void Cancel_WhenNotPending_ThrowsDomainException() {
    // Arrange
    var order = new OrderBuilder().Build();
    order.MarkAsPaid("billing@example.com");

    // Act
    var act = () => order.Cancel();

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Chỉ có thể hủy đơn khi đơn ở trạng thái Chờ");
  }

  [Fact]
  public void Refund_WhenConfirmed_ChangesStatusToRefunded() {
    // Arrange
    var order = new OrderBuilder().Build();
    order.MarkAsPaid("billing@example.com");

    // Act
    order.Refund();

    // Assert
    order.Status.Should().Be(OrderStatus.Refunded);
  }

  [Fact]
  public void Refund_WhenNotConfirmed_ThrowsDomainException() {
    // Arrange
    var order = new OrderBuilder().Build();

    // Act
    var act = () => order.Refund();

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Chỉ có thể hoàn đơn khi đơn ở trạng thái Đã xác nhận");
  }
}
