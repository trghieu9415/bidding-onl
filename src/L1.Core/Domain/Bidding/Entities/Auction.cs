using L1.Core.Base.Entity;
using L1.Core.Domain.Bidding.Enums;
using L1.Core.Domain.Bidding.Events;
using L1.Core.Domain.Bidding.ValueObjects;
using L1.Core.Exceptions;

namespace L1.Core.Domain.Bidding.Entities;

public class Auction : AggregateRoot {
  private readonly List<Bid> _bids = [];
  private Auction() {}
  public Guid SessionId { get; private set; }
  public Guid CatalogItemId { get; private set; }
  public AuctionStatus Status { get; private set; } = AuctionStatus.Scheduled;
  public decimal CurrentPrice { get; private set; }
  public int TotalBids { get; private set; }
  public Guid? LastBidId { get; private set; }
  public Guid? LastBidderId { get; private set; }
  public string? LastBidderName { get; private set; }
  public Guid OwnerId { get; private set; }
  public Guid? WinningBidId { get; private set; }
  public DateTime? WinningAt { get; private set; }
  public AuctionRules Rules { get; private set; } = null!;
  public IReadOnlyCollection<Bid> Bids => _bids.AsReadOnly();

  public static Auction Create(
    Guid catalogItemId, Guid sessionId,
    decimal startingPrice, decimal stepPrice, decimal reversePrice
  ) {
    return new Auction {
      CatalogItemId = catalogItemId,
      SessionId = sessionId,
      CurrentPrice = startingPrice,
      Rules = new AuctionRules(stepPrice, reversePrice)
    };
  }

  public Auction SetOwnerId(Guid ownerId) {
    OwnerId = ownerId;
    return this;
  }

  public void UpdateRules(decimal stepPrice, decimal reservePrice) {
    if (Status != AuctionStatus.Scheduled) {
      throw new DomainException("Chỉ được sửa quy tắc khi đấu giá chưa bắt đầu.");
    }

    Rules = new AuctionRules(stepPrice, reservePrice);
  }

  public Bid PlaceBid(Guid bidderId, string bidderName, decimal amount) {
    if (Status != AuctionStatus.Active) {
      throw new DomainException("Chỉ có thể đặt giá khi đấu giá đang diễn ra.");
    }

    var minimumNextBid = TotalBids == 0 ? CurrentPrice : CurrentPrice + Rules.StepPrice;
    if (amount < minimumNextBid) {
      throw new DomainException($"Giá đặt phải tối thiểu là {minimumNextBid}");
    }

    var previousBidderId = LastBidderId;

    var bid = Bid.Create(this, bidderId, amount);
    _bids.Add(bid);
    CurrentPrice = amount;
    LastBidId = bid.Id;
    LastBidderId = bidderId;
    LastBidderName = bidderName;
    TotalBids++;

    AddDomainEvent(new BidPlacedEvent(Id, bidderId, bidderName, amount));
    if (previousBidderId.HasValue && previousBidderId != bidderId) {
      AddDomainEvent(new OutbidEvent(Id, previousBidderId.Value, amount));
    }

    return bid;
  }

  public void End() {
    if (Status != AuctionStatus.Active) {
      throw new DomainException("Không thể kết thúc phiên đấu giá đang diễn ra.");
    }

    var isSold = TotalBids > 0 && CurrentPrice >= Rules.ReservePrice;
    if (isSold) {
      WinningBidId = LastBidId;
      WinningAt = DateTime.UtcNow;
      Status = AuctionStatus.EndedSold;
    } else {
      Status = AuctionStatus.EndedUnsold;
    }

    AddDomainEvent(new AuctionEndedEvent(Id, WinningBidId, CurrentPrice, OwnerId, isSold));
  }

  public void Start() {
    if (Status != AuctionStatus.Scheduled) {
      throw new DomainException("Phiên đấu giá không ở trạng thái được lên lịch");
    }

    Status = AuctionStatus.Active;
    AddDomainEvent(new AuctionStartedEvent(Id, CatalogItemId, OwnerId));
  }

  public void Cancel() {
    if (Status != AuctionStatus.Scheduled) {
      throw new DomainException("Không thể hủy phiên đấu giá đang diễn ra");
    }

    Status = AuctionStatus.Canceled;
  }

  public void Paid(bool isPaid) {
    if (Status != AuctionStatus.EndedSold) {
      throw new DomainException("Cuộc đấu giá chưa ở trạng thái chờ thanh toán");
    }

    Status = isPaid ? AuctionStatus.Completed : AuctionStatus.Canceled;
  }
}
