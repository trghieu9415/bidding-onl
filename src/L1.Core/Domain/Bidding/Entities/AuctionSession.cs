using System.ComponentModel.DataAnnotations;
using L1.Core.Base.Entity;
using L1.Core.Domain.Bidding.Enums;
using L1.Core.Domain.Bidding.Events;
using L1.Core.Domain.Bidding.ValueObjects;
using L1.Core.Exceptions;

namespace L1.Core.Domain.Bidding.Entities;

public class AuctionSession : AggregateRoot {
  private readonly List<Guid> _auctionIds = [];
  private AuctionSession() {}
  [Required] public string Title { get; private set; } = null!;
  public AuctionTimeFrame TimeFrame { get; private set; } = null!;
  public SessionStatus Status { get; private set; } = SessionStatus.Draft;
  public IReadOnlyCollection<Guid> AuctionIds => _auctionIds.AsReadOnly();

  public static AuctionSession Create(string title, DateTime startTime, DateTime endTime) {
    return new AuctionSession {
      Title = title
    }.SetTimeFrame(startTime, endTime);
  }

  public AuctionSession Update(string title) {
    Title = title;
    return this;
  }

  public AuctionSession SetTimeFrame(DateTime startTime, DateTime endTime) {
    AuctionTimeFrame.Validate(startTime, endTime);

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
    if (Status != SessionStatus.Draft) {
      throw new DomainException("Trạng thái không phù hợp để công khai phiên đấu giá");
    }

    Status = SessionStatus.Published;
    AddDomainEvent(new SessionPublishedEvent(Id, Title, TimeFrame.StartTime, TimeFrame.EndTime));
  }

  public void Start() {
    Status = SessionStatus.Live;
  }

  public void Close() {
    Status = SessionStatus.Closed;
  }
}
