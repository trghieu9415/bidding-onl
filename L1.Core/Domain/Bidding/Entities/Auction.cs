using L1.Core.Base.Entity;
using L1.Core.Base.Exception;
using L1.Core.Domain.Bidding.Enums;
using L1.Core.Domain.Bidding.Events;
using L1.Core.Domain.Bidding.ValueObjects;

namespace L1.Core.Domain.Bidding.Entities;

public class Auction : AggregateRoot {
  private readonly List<Bid> _bids = [];
  private Auction() {}
  public Guid CatalogItemId { get; private set; }
  public AuctionStatus Status { get; private set; } = AuctionStatus.Scheduled;
  public decimal CurrentPrice { get; private set; }
  public Guid? WinningBidId { get; private set; }
  public DateTime? WinningAt { get; private set; }
  public AuctionRules Rules { get; private set; } = null!;
  public IReadOnlyCollection<Bid> Bids => _bids.AsReadOnly();

  public static Auction Create(Guid catalogItemId, decimal startingPrice, decimal stepPrice, decimal reversePrice) {
    return new Auction {
      CatalogItemId = catalogItemId,
      CurrentPrice = startingPrice,
      Rules = new AuctionRules(stepPrice, reversePrice)
    };
  }

  public void UpdateRules(decimal stepPrice, decimal reservePrice) {
    if (Status != AuctionStatus.Scheduled) {
      throw new DomainException("Chỉ được sửa quy tắc khi đấu giá chưa bắt đầu.");
    }

    Rules = new AuctionRules(stepPrice, reservePrice);
  }

  public void PlaceBid(Guid bidderId, decimal amount) {
    if (Status != AuctionStatus.Active) {
      throw new DomainException("Chỉ có thể đặt giá khi đấu giá đang diễn ra.");
    }

    var minimumNextBid = _bids.Count == 0
      ? Math.Max(CurrentPrice, Rules.ReservePrice)
      : CurrentPrice + Rules.StepPrice;

    if (amount < minimumNextBid) {
      throw new DomainException($"Giá đặt phải tối thiểu là {minimumNextBid}");
    }

    var previousBidderId = _bids.OrderByDescending(x => x.Amount).FirstOrDefault()?.BidderId;

    var bid = Bid.Create(this, bidderId, amount);
    _bids.Add(bid);
    CurrentPrice = amount;

    AddDomainEvent(new BidPlacedEvent(Id, bidderId, amount));
    if (previousBidderId.HasValue && previousBidderId != bidderId) {
      AddDomainEvent(new OutbidEvent(Id, previousBidderId.Value, amount));
    }
  }

  public void End() {
    if (Status != AuctionStatus.Active) {
      throw new DomainException("Không thể kết thúc phiên đấu giá đang diễn ra.");
    }

    var isSold = _bids.Count > 0 || CurrentPrice <= Rules.ReservePrice;
    if (isSold) {
      WinningBidId = _bids.OrderByDescending(x => x.Amount).First().Id;
      WinningAt = DateTime.Now;
      Status = AuctionStatus.EndedSold;
    } else {
      Status = AuctionStatus.EndedUnsold;
    }

    AddDomainEvent(new AuctionEndedEvent(Id, WinningBidId, CurrentPrice, isSold));
  }

  public void Start() {
    if (Status != AuctionStatus.Scheduled) {
      throw new DomainException("Phiên đấu giá không ở trạng thái được lên lịch");
    }

    Status = AuctionStatus.Active;
    AddDomainEvent(new AuctionStartedEvent(Id));
  }

  public void Cancel() {
    if (Status != AuctionStatus.Scheduled) {
      throw new DomainException("Không thể hủy phiên đấu giá đang diễn ra");
    }

    Status = AuctionStatus.Canceled;
  }

  public void Paid(bool isPaid) {
    if (Status != AuctionStatus.EndedSold) {
      throw new DomainException("Phiên đấu giá chưa kết thúc hợp lệ");
    }

    Status = isPaid ? AuctionStatus.Completed : AuctionStatus.Canceled;
  }
}
