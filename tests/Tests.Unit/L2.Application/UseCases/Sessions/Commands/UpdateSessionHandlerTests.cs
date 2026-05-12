using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.Sessions.Commands.UpdateSession;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Sessions.Commands;

public class UpdateSessionHandlerTests {
  private readonly IRepository<AuctionSession> _repository = Substitute.For<IRepository<AuctionSession>>();
  private readonly UpdateSessionHandler _sut;

  public UpdateSessionHandlerTests() {
    _sut = new UpdateSessionHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_SessionNotFound() {
    var request = new UpdateSessionCommand(Guid.NewGuid(), new UpdateSessionRequest("New", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2)));

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns((AuctionSession?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Không tìm thấy phiên đấu giá");
  }

  [Fact]
  public async Task Handle_Should_UpdateSession_And_ReturnTrue() {
    var session = new AuctionSessionBuilder().Build();
    var start = DateTime.UtcNow.AddHours(3);
    var end = DateTime.UtcNow.AddHours(4);
    var request = new UpdateSessionCommand(session.Id, new UpdateSessionRequest("Updated", start, end));

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns(session);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    session.Title.Should().Be("Updated");
    session.TimeFrame.StartTime.Should().Be(start);
    session.TimeFrame.EndTime.Should().Be(end);
    await _repository.Received(1).UpdateAsync(session, CancellationToken.None);
  }
}
