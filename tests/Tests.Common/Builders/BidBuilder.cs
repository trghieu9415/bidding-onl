using System.Diagnostics.CodeAnalysis;
using L1.Core.Domain.Bidding.Entities;

namespace Tests.Common.Builders;

[ExcludeFromCodeCoverage]
public class BidBuilder {
  private decimal _amount = 120m;
  private Auction _auction = new AuctionBuilder().Build();
  private Guid _bidderId = Guid.NewGuid();

  public BidBuilder WithAuction(Auction auction) {
    _auction = auction;
    return this;
  }

  public BidBuilder WithBidderId(Guid bidderId) {
    _bidderId = bidderId;
    return this;
  }

  public BidBuilder WithAmount(decimal amount) {
    _amount = amount;
    return this;
  }

  public Bid Build() {
    return Bid.Create(_auction, _bidderId, _amount);
  }
}
