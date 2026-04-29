using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L1.Core.Domain.Transaction.Events;
using L1.Core.Domain.Transaction.ValueObjects;
using L1.Core.Exceptions;
using Xunit;

namespace Tests.Unit.L1.Core.Transaction;

public class OrderTests {
  private static readonly Address ShippingAddress = new("John Doe", "0123456789", "123 Auction Street");

  [Fact]
  public void Create_ValidParameters_InitializesPendingOrderAndRaisesCreatedEvent() {
    var bidderId = Guid.NewGuid();
    var auctionId = Guid.NewGuid();
    var catalogId = Guid.NewGuid();

    var order = Order.Create(
      bidderId,
      "John Doe",
      "john@example.com",
      auctionId,
      catalogId,
      "Laptop",
      "main.png",
      ShippingAddress
    );

    Assert.Equal(bidderId, order.BidderId);
    Assert.Equal("John Doe", order.BidderName);
    Assert.Equal("john@example.com", order.BidderEmail);
    Assert.Equal(auctionId, order.AuctionId);
    Assert.Equal(catalogId, order.CatalogId);
    Assert.Equal("Laptop", order.CatalogName);
    Assert.Equal("main.png", order.CatalogImage);
    Assert.Equal(ShippingAddress, order.Address);
    Assert.Equal(OrderStatus.Pending, order.Status);
    Assert.Equal(0m, order.Price);

    var createdEvent = Assert.IsType<OrderCreatedEvent>(Assert.Single(order.DomainEvents));
    Assert.Equal(order.Id, createdEvent.AggregateId);
    Assert.Equal(bidderId, createdEvent.BidderId);
    Assert.Equal(auctionId, createdEvent.AuctionId);
  }

  [Fact]
  public void MarkAsPaid_WhenPending_ChangesStatusAndRaisesCompletedEvent() {
    var order = CreateOrder();
    order.ClearEvents();

    order.MarkAsPaid("billing@example.com");

    Assert.Equal(OrderStatus.Confirmed, order.Status);
    var completedEvent = Assert.IsType<OrderCompletedEvent>(Assert.Single(order.DomainEvents));
    Assert.Equal(order.Id, completedEvent.AggregateId);
    Assert.Equal(order.BidderId, completedEvent.BidderId);
    Assert.Equal(order.AuctionId, completedEvent.AuctionId);
    Assert.Equal(order.BidderName, completedEvent.BidderName);
    Assert.Equal("billing@example.com", completedEvent.BidderEmail);
  }

  [Fact]
  public void MarkAsPaid_WhenNotPending_ThrowsDomainException() {
    var order = CreateOrder();
    order.Cancel();

    var exception = Assert.Throws<DomainException>(() => order.MarkAsPaid("billing@example.com"));

    Assert.Equal("Chỉ có thể thanh toán đơn khi đơn ở trạng thái Chờ", exception.Message);
  }

  [Fact]
  public void Cancel_WhenPending_ChangesStatusAndRaisesCanceledEvent() {
    var order = CreateOrder();
    order.ClearEvents();

    order.Cancel();

    Assert.Equal(OrderStatus.Canceled, order.Status);
    var canceledEvent = Assert.IsType<OrderCanceledEvent>(Assert.Single(order.DomainEvents));
    Assert.Equal(order.Id, canceledEvent.AggregateId);
    Assert.Equal(order.BidderId, canceledEvent.CustomerId);
    Assert.Equal(order.AuctionId, canceledEvent.AuctionId);
  }

  [Fact]
  public void Cancel_WhenNotPending_ThrowsDomainException() {
    var order = CreateOrder();
    order.MarkAsPaid("billing@example.com");

    var exception = Assert.Throws<DomainException>(() => order.Cancel());

    Assert.Equal("Chỉ có thể hủy đơn khi đơn ở trạng thái Chờ", exception.Message);
  }

  [Fact]
  public void Refund_WhenConfirmed_ChangesStatusToRefunded() {
    var order = CreateOrder();
    order.MarkAsPaid("billing@example.com");

    order.Refund();

    Assert.Equal(OrderStatus.Refunded, order.Status);
  }

  [Fact]
  public void Refund_WhenNotConfirmed_ThrowsDomainException() {
    var order = CreateOrder();

    var exception = Assert.Throws<DomainException>(() => order.Refund());

    Assert.Equal("Chỉ có thể hoàn đơn khi đơn ở trạng thái Đã xác nhận", exception.Message);
  }

  private static Order CreateOrder() {
    return Order.Create(
      Guid.NewGuid(),
      "John Doe",
      "john@example.com",
      Guid.NewGuid(),
      Guid.NewGuid(),
      "Laptop",
      "main.png",
      ShippingAddress
    );
  }
}
