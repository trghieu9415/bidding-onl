using System.ComponentModel.DataAnnotations;
using L1.Core.Base.Entity;
using L1.Core.Base.Exception;
using L1.Core.Domain.Bidding.Enums;
using L1.Core.Domain.Bidding.ValueObjects;

namespace L1.Core.Domain.Bidding.Entities;

public class AuctionSession : AggregateRoot {
  private readonly List<Guid> _auctionIds = [];
  private AuctionSession() {}
  [Required] public string Title { get; private set; } = null!;
  public AuctionTimeFrame? TimeFrame { get; private set; }
  public SessionStatus Status { get; private set; } = SessionStatus.Draft;
  public IReadOnlyCollection<Guid> AuctionIds => _auctionIds.AsReadOnly();

  public static AuctionSession? Create(string title) {
    return new AuctionSession {
      Title = title
    };
  }

  public AuctionSession SetTimeFrame(DateTime startTime, DateTime endTime) {
    if (startTime >= endTime) {
      throw new DomainException("Thời gian bắt đầu phải trước thời gian kết thúc.");
    }

    if (Status != SessionStatus.Draft && Status != SessionStatus.Published) {
      throw new DomainException("Không thể thay đổi thời gian khi phiên đã diễn ra.");
    }

    TimeFrame = new AuctionTimeFrame(startTime, endTime);
    return this;
  }


  public AuctionSession SyncAuctions(ICollection<Guid> auctionIds) {
    if (Status == SessionStatus.Closed) {
      throw new DomainException("Phiên đấu giá đã đóng.");
    }

    _auctionIds.Clear();
    _auctionIds.AddRange(auctionIds);
    return this;
  }

  public void Publish() {
    Status = SessionStatus.Published;
  }

  public void Start() {
    Status = SessionStatus.Live;
  }

  public void Close() {
    Status = SessionStatus.Closed;
  }
}