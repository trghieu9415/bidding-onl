using L1.Core.Base.Entity;
using L1.Core.Base.Exception;
using L1.Core.Domain.Bidding.Enums;
using L1.Core.Domain.Bidding.ValueObjects;

namespace L1.Core.Domain.Bidding.Entities;

public class Auction : AggregateRoot {
  private readonly List<Bid> _bids = [];
  private Auction() {}
  public Guid CatalogItemId { get; private set; }
  public AuctionStatus Status { get; private set; } = AuctionStatus.Scheduled;
  public decimal CurrentPrice { get; private set; }
  public Guid? WinningBidId { get; private set; }
  public AuctionRules Rules { get; private set; } = null!;
  public IReadOnlyCollection<Bid> Bids => _bids.AsReadOnly();

  public static Auction Create(Guid catalogItemId, decimal stepPrice, decimal reversePrice) {
    return new Auction {
      CatalogItemId = catalogItemId,
      Rules = new AuctionRules(stepPrice, reversePrice)
    };
  }

  public void PlaceBid(Guid bidderId, decimal amount) {
    if (WinningBidId.HasValue) {
      throw new DomainException("Đã có người ____");
    }

    if (amount <= CurrentPrice) {
      throw new DomainException("Giá đặt ___");
    }

    _bids.Add(Bid.Create(this, bidderId, amount));
    CurrentPrice = amount;
  }

  public void End() {
    if (_bids.Count == 0) {
      Status = AuctionStatus.EndedUnsold;
    } else {
      WinningBidId = _bids.Last().Id;
      Status = AuctionStatus.EndedSold;
    }
  }

  public void Start() {
    Status = AuctionStatus.Active;
  }

  public void Cancel() {
    if (Status != AuctionStatus.Scheduled) {
      throw new DomainException("Không thể hủy __ trạng thái hiện tại");
    }

    Status = AuctionStatus.Canceled;
  }
}