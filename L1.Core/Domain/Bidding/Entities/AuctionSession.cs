using System.ComponentModel.DataAnnotations;
using L1.Core.Base.Entity;
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
    TimeFrame = new AuctionTimeFrame(startTime, endTime);
    return this;
  }

  public AuctionSession SyncAuctions(ICollection<Guid> auctionIds) {
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