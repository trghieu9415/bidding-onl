using System.Diagnostics.CodeAnalysis;
using L1.Core.Domain.Bidding.Entities;

namespace Tests.Common.Builders;

[ExcludeFromCodeCoverage]
public class AuctionSessionBuilder {
  private readonly List<Guid> _auctionIds = [];
  private DateTime _endTime = DateTime.UtcNow.AddHours(2);
  private DateTime _startTime = DateTime.UtcNow.AddHours(1);
  private string _title = "Default Session";

  public AuctionSessionBuilder WithTitle(string title) {
    _title = title;
    return this;
  }

  public AuctionSessionBuilder InThePast() {
    _startTime = DateTime.UtcNow.AddHours(-2);
    _endTime = DateTime.UtcNow.AddHours(-1);
    return this;
  }

  public AuctionSession Build() {
    var session = AuctionSession.Create(_title, _startTime, _endTime);
    if (_auctionIds.Count > 0) {
      session.SyncAuctions(_auctionIds);
    }

    return session;
  }
}
