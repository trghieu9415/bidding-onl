using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.ValueObjects;

namespace Tests.Common.Builders;

public class OrderBuilder {
  private Address _address = new("John Doe", "0123456789", "123 Auction Street");
  private Guid _auctionId = Guid.NewGuid();
  private readonly string _bidderEmail = "john@example.com";
  private Guid _bidderId = Guid.NewGuid();
  private readonly string _bidderName = "John Doe";
  private Guid _catalogId = Guid.NewGuid();
  private readonly string _catalogImage = "main.png";
  private readonly string _catalogName = "Laptop";

  public OrderBuilder WithBidderId(Guid bidderId) {
    _bidderId = bidderId;
    return this;
  }

  public OrderBuilder WithAuctionId(Guid auctionId) {
    _auctionId = auctionId;
    return this;
  }

  public OrderBuilder WithCatalogId(Guid catalogId) {
    _catalogId = catalogId;
    return this;
  }

  public OrderBuilder WithAddress(Address address) {
    _address = address;
    return this;
  }

  public Order Build() {
    return Order.Create(
      _bidderId,
      _bidderName,
      _bidderEmail,
      _auctionId,
      _catalogId,
      _catalogName,
      _catalogImage,
      _address
    );
  }
}
