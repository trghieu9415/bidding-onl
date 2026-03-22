using L1.Core.Base.Entity;
using L1.Core.Domain.Transaction.Enums;
using L1.Core.Domain.Transaction.Events;
using L1.Core.Domain.Transaction.ValueObjects;
using L1.Core.Exceptions;

namespace L1.Core.Domain.Transaction.Entities;

public class Order : AggregateRoot {
  private Order() {}
  public Guid BidderId { get; private set; }
  public string BidderName { get; private set; } = null!;
  public Guid AuctionId { get; private set; }
  public Guid CatalogId { get; private set; }
  public string CatalogName { get; private set; } = null!;
  public string CatalogImage { get; private set; } = null!;
  public Address Address { get; private set; } = null!;

  public OrderStatus Status { get; private set; } = OrderStatus.Pending;
  public decimal Price { get; private set; }

  public static Order Create(
    Guid customerId, string customerName,
    Guid auctionId, Guid catalogId,
    string catalogName, string catalogImage,
    Address shippingAddress
  ) {
    var order = new Order {
      BidderId = customerId,
      BidderName = customerName,
      AuctionId = auctionId,
      CatalogId = catalogId,
      CatalogName = catalogName,
      CatalogImage = catalogImage,
      Address = shippingAddress
    };
    order.AddDomainEvent(new OrderCreatedEvent(
      order.Id,
      order.BidderId,
      order.AuctionId
    ));
    return order;
  }


  public void MarkAsPaid(string bidderEmail) {
    if (Status != OrderStatus.Pending) {
      throw new DomainException("Chỉ có thể thanh toán đơn khi đơn ở trạng thái Chờ");
    }

    Status = OrderStatus.Confirmed;
    AddDomainEvent(new OrderCompletedEvent(
      Id, BidderId, AuctionId, BidderName, bidderEmail
    ));
  }

  public void Cancel() {
    if (Status != OrderStatus.Pending) {
      throw new DomainException("Chỉ có thể hủy đơn khi đơn ở trạng thái Chờ");
    }

    Status = OrderStatus.Canceled;
    AddDomainEvent(new OrderCanceledEvent(
      Id, BidderId, AuctionId
    ));
  }

  public void Refund() {
    if (Status != OrderStatus.Confirmed) {
      throw new DomainException("Chỉ có thể hoàn đơn khi đơn ở trạng thái Đã xác nhận");
    }

    Status = OrderStatus.Refunded;
  }
}
