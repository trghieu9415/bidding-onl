using System.Diagnostics.CodeAnalysis;
using L1.Core.Domain.Bidding.Entities;

namespace Tests.Common.Builders;

[ExcludeFromCodeCoverage]
public class AuctionBuilder {
  private Guid _catalogItemId = Guid.NewGuid();
  private decimal _currentPrice = 100m;
  private Guid? _ownerId = Guid.NewGuid();
  private decimal _reservePrice = 500m;
  private Guid _sessionId = Guid.NewGuid();
  private decimal _stepPrice = 10m;

  public AuctionBuilder WithCatalogItemId(Guid catalogItemId) {
    _catalogItemId = catalogItemId;
    return this;
  }

  public AuctionBuilder WithSessionId(Guid sessionId) {
    _sessionId = sessionId;
    return this;
  }

  public AuctionBuilder WithPrices(decimal currentPrice, decimal stepPrice, decimal reservePrice) {
    _currentPrice = currentPrice;
    _stepPrice = stepPrice;
    _reservePrice = reservePrice;
    return this;
  }

  public AuctionBuilder WithOwnerId(Guid? ownerId) {
    _ownerId = ownerId;
    return this;
  }

  public Auction Build() {
    var auction = Auction.Create(_catalogItemId, _sessionId, _currentPrice, _stepPrice, _reservePrice);
    if (_ownerId.HasValue) {
      auction.SetOwnerId(_ownerId.Value);
    }

    return auction;
  }
}
