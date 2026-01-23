using L1.Core.Base.Entity;

namespace L1.Core.Domain.Bidding.Entities;

public class Bid : BaseEntity {
  private Bid() {}
  public Guid AuctionId { get; private set; }
  public Auction Auction { get; private set; } = null!;

  public decimal Amount { get; private set; }
  public Guid BidderId { get; private set; }
  public DateTime TimePoint { get; private set; } = DateTime.Now;

  public static Bid Create(Auction auction, Guid bidderId, decimal amount) {
    return new Bid {
      AuctionId = auction.Id,
      Auction = auction,
      BidderId = bidderId,
      Amount = amount
    };
  }
}
