using FluentAssertions;
using L1.Core.Domain.Bidding.Enums;
using L1.Core.Domain.Bidding.Events;
using L1.Core.Exceptions;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.Entities;

public class AuctionSessionTests {
  [Fact]
  public void Create_ValidParameters_InitializesDraftSession() {
    // Arrange
    var builder = new AuctionSessionBuilder().WithTitle("Morning Session");

    // Act
    var session = builder.Build();

    // Assert
    session.Title.Should().Be("Morning Session");
    session.Status.Should().Be(SessionStatus.Draft);
    session.TimeFrame.StartTime.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), TimeSpan.FromSeconds(5));
    session.TimeFrame.EndTime.Should().BeCloseTo(DateTime.UtcNow.AddHours(2), TimeSpan.FromSeconds(5));
    session.AuctionIds.Should().BeEmpty();
  }

  [Fact]
  public void Update_ChangesTitleAndReturnsSameSession() {
    // Arrange
    var session = new AuctionSessionBuilder().WithTitle("Morning Session").Build();

    // Act
    var returnedSession = session.Update("Evening Session");

    // Assert
    returnedSession.Should().BeSameAs(session);
    session.Title.Should().Be("Evening Session");
  }

  [Fact]
  public void SetTimeFrame_WhenDraft_UpdatesTimeFrame() {
    // Arrange
    var session = new AuctionSessionBuilder().Build();
    var newStart = DateTime.UtcNow.AddHours(3);
    var newEnd = DateTime.UtcNow.AddHours(4);

    // Act
    var returnedSession = session.SetTimeFrame(newStart, newEnd);

    // Assert
    returnedSession.Should().BeSameAs(session);
    session.TimeFrame.StartTime.Should().Be(newStart);
    session.TimeFrame.EndTime.Should().Be(newEnd);
  }

  [Fact]
  public void SetTimeFrame_WhenPublished_UpdatesTimeFrame() {
    // Arrange
    var session = new AuctionSessionBuilder().Build();
    session.Publish();
    session.ClearEvents();
    var newStart = DateTime.UtcNow.AddHours(5);
    var newEnd = DateTime.UtcNow.AddHours(6);

    // Act
    session.SetTimeFrame(newStart, newEnd);

    // Assert
    session.TimeFrame.StartTime.Should().Be(newStart);
    session.TimeFrame.EndTime.Should().Be(newEnd);
  }

  [Fact]
  public void SetTimeFrame_WhenSessionIsLive_ThrowsDomainException() {
    // Arrange
    var session = new AuctionSessionBuilder().Build();
    session.Start();

    // Act
    Action act = () => session.SetTimeFrame(DateTime.UtcNow.AddHours(3), DateTime.UtcNow.AddHours(4));

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Không thể thay đổi thời gian khi phiên đã diễn ra.");
  }

  [Fact]
  public void SetTimeFrame_WhenEndTimeIsNotAfterStartTime_ThrowsDomainException() {
    // Arrange
    var session = new AuctionSessionBuilder().Build();
    var startTime = DateTime.UtcNow.AddHours(3);

    // Act
    Action act = () => session.SetTimeFrame(startTime, startTime);

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Thời gian bắt đầu phải trước thời gian kết thúc.");
  }

  [Fact]
  public void SetTimeFrame_WhenEndTimeIsInPast_ThrowsDomainException() {
    // Arrange
    var session = new AuctionSessionBuilder().Build();

    // Act
    Action act = () => session.SetTimeFrame(DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1));

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Thời gian kết thúc không được ở trong quá khứ.");
  }

  [Fact]
  public void SyncAuctions_ReplacesAuctionIdsAndReturnsSameSession() {
    // Arrange
    var session = new AuctionSessionBuilder().Build();
    session.SyncAuctions([Guid.NewGuid()]);
    var auctionIds = new[] { Guid.NewGuid(), Guid.NewGuid() };

    // Act
    var returnedSession = session.SyncAuctions(auctionIds);

    // Assert
    returnedSession.Should().BeSameAs(session);
    session.AuctionIds.Should().BeEquivalentTo(auctionIds);
  }

  [Fact]
  public void SyncAuctions_WhenClosed_ThrowsDomainException() {
    // Arrange
    var session = new AuctionSessionBuilder().Build();
    session.Close();

    // Act
    Action act = () => session.SyncAuctions([Guid.NewGuid()]);

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Phiên đấu giá đã đóng.");
  }

  [Fact]
  public void Publish_WhenDraft_ChangesStatusAndRaisesEvent() {
    // Arrange
    var session = new AuctionSessionBuilder().WithTitle("Morning Session").Build();

    // Act
    session.Publish();

    // Assert
    session.Status.Should().Be(SessionStatus.Published);
    var publishedEvent = session.DomainEvents.Should().ContainSingle().Subject.As<SessionPublishedEvent>();
    publishedEvent.AggregateId.Should().Be(session.Id);
    publishedEvent.Title.Should().Be("Morning Session");
    publishedEvent.StartTime.Should().Be(session.TimeFrame.StartTime);
    publishedEvent.EndTime.Should().Be(session.TimeFrame.EndTime);
  }

  [Fact]
  public void Publish_WhenNotDraft_ThrowsDomainException() {
    // Arrange
    var session = new AuctionSessionBuilder().Build();
    session.Publish();

    // Act
    var act = () => session.Publish();

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Trạng thái không phù hợp để công khai phiên đấu giá");
  }

  [Fact]
  public void Start_ChangesStatusToLive() {
    // Arrange
    var session = new AuctionSessionBuilder().Build();

    // Act
    session.Start();

    // Assert
    session.Status.Should().Be(SessionStatus.Live);
  }

  [Fact]
  public void Close_ChangesStatusToClosed() {
    // Arrange
    var session = new AuctionSessionBuilder().Build();

    // Act
    session.Close();

    // Assert
    session.Status.Should().Be(SessionStatus.Closed);
  }
}
