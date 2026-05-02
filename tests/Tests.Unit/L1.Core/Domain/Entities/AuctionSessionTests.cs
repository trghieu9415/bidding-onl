using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L1.Core.Domain.Bidding.Events;
using L1.Core.Exceptions;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.Entities;

public class AuctionSessionTests {
  [Fact]
  public void Create_ValidParameters_InitializesDraftSession() {
    var startTime = DateTime.UtcNow.AddHours(1);
    var endTime = DateTime.UtcNow.AddHours(2);

    var session = AuctionSession.Create("Morning Session", startTime, endTime);

    Assert.Equal("Morning Session", session.Title);
    Assert.Equal(SessionStatus.Draft, session.Status);
    Assert.Equal(startTime, session.TimeFrame.StartTime);
    Assert.Equal(endTime, session.TimeFrame.EndTime);
    Assert.Empty(session.AuctionIds);
  }

  [Fact]
  public void Update_ChangesTitleAndReturnsSameSession() {
    var session = AuctionSession.Create("Morning Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));

    var returnedSession = session.Update("Evening Session");

    Assert.Same(session, returnedSession);
    Assert.Equal("Evening Session", session.Title);
  }

  [Fact]
  public void SetTimeFrame_WhenDraft_UpdatesTimeFrame() {
    var session = AuctionSession.Create("Morning Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
    var newStart = DateTime.UtcNow.AddHours(3);
    var newEnd = DateTime.UtcNow.AddHours(4);

    var returnedSession = session.SetTimeFrame(newStart, newEnd);

    Assert.Same(session, returnedSession);
    Assert.Equal(newStart, session.TimeFrame.StartTime);
    Assert.Equal(newEnd, session.TimeFrame.EndTime);
  }

  [Fact]
  public void SetTimeFrame_WhenPublished_UpdatesTimeFrame() {
    var session = AuctionSession.Create("Morning Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
    session.Publish();
    session.ClearEvents();
    var newStart = DateTime.UtcNow.AddHours(5);
    var newEnd = DateTime.UtcNow.AddHours(6);

    session.SetTimeFrame(newStart, newEnd);

    Assert.Equal(newStart, session.TimeFrame.StartTime);
    Assert.Equal(newEnd, session.TimeFrame.EndTime);
  }

  [Fact]
  public void SetTimeFrame_WhenSessionIsLive_ThrowsDomainException() {
    var session = AuctionSession.Create("Morning Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
    session.Start();

    var exception = Assert.Throws<DomainException>(() =>
      session.SetTimeFrame(DateTime.UtcNow.AddHours(3), DateTime.UtcNow.AddHours(4)));

    Assert.Equal("Không thể thay đổi thời gian khi phiên đã diễn ra.", exception.Message);
  }

  [Fact]
  public void SetTimeFrame_WhenEndTimeIsNotAfterStartTime_ThrowsDomainException() {
    var session = AuctionSession.Create("Morning Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
    var startTime = DateTime.UtcNow.AddHours(3);

    var exception = Assert.Throws<DomainException>(() => session.SetTimeFrame(startTime, startTime));

    Assert.Equal("Thời gian bắt đầu phải trước thời gian kết thúc.", exception.Message);
  }

  [Fact]
  public void SetTimeFrame_WhenEndTimeIsInPast_ThrowsDomainException() {
    var session = AuctionSession.Create("Morning Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));

    var exception = Assert.Throws<DomainException>(() =>
      session.SetTimeFrame(DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1)));

    Assert.Equal("Thời gian kết thúc không được ở trong quá khứ.", exception.Message);
  }

  [Fact]
  public void SyncAuctions_ReplacesAuctionIdsAndReturnsSameSession() {
    var session = AuctionSession.Create("Morning Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
    session.SyncAuctions([Guid.NewGuid()]);
    var auctionIds = new[] { Guid.NewGuid(), Guid.NewGuid() };

    var returnedSession = session.SyncAuctions(auctionIds);

    Assert.Same(session, returnedSession);
    Assert.Equal(auctionIds, session.AuctionIds);
  }

  [Fact]
  public void SyncAuctions_WhenClosed_ThrowsDomainException() {
    var session = AuctionSession.Create("Morning Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
    session.Close();

    var exception = Assert.Throws<DomainException>(() => session.SyncAuctions([Guid.NewGuid()]));

    Assert.Equal("Phiên đấu giá đã đóng.", exception.Message);
  }

  [Fact]
  public void Publish_WhenDraft_ChangesStatusAndRaisesEvent() {
    var startTime = DateTime.UtcNow.AddHours(1);
    var endTime = DateTime.UtcNow.AddHours(2);
    var session = AuctionSession.Create("Morning Session", startTime, endTime);

    session.Publish();

    Assert.Equal(SessionStatus.Published, session.Status);
    var publishedEvent = Assert.IsType<SessionPublishedEvent>(Assert.Single(session.DomainEvents));
    Assert.Equal(session.Id, publishedEvent.AggregateId);
    Assert.Equal("Morning Session", publishedEvent.Title);
    Assert.Equal(startTime, publishedEvent.StartTime);
    Assert.Equal(endTime, publishedEvent.EndTime);
  }

  [Fact]
  public void Publish_WhenNotDraft_ThrowsDomainException() {
    var session = AuctionSession.Create("Morning Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
    session.Publish();

    var exception = Assert.Throws<DomainException>(() => session.Publish());

    Assert.Equal("Trạng thái không phù hợp để công khai phiên đấu giá", exception.Message);
  }

  [Fact]
  public void Start_ChangesStatusToLive() {
    var session = AuctionSession.Create("Morning Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));

    session.Start();

    Assert.Equal(SessionStatus.Live, session.Status);
  }

  [Fact]
  public void Close_ChangesStatusToClosed() {
    var session = AuctionSession.Create("Morning Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));

    session.Close();

    Assert.Equal(SessionStatus.Closed, session.Status);
  }
}
