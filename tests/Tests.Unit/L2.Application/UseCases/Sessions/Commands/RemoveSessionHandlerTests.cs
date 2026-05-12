using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.Repositories;
using L2.Application.UseCases.Sessions.Commands.RemoveSession;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Sessions.Commands;

public class RemoveSessionHandlerTests {
  private readonly IRepository<AuctionSession> _repository = Substitute.For<IRepository<AuctionSession>>();
  private readonly RemoveSessionHandler _sut;

  public RemoveSessionHandlerTests() {
    _sut = new RemoveSessionHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_DeleteSession_And_ReturnTrue() {
    var request = new RemoveSessionCommand(Guid.NewGuid());

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    await _repository.Received(1).DeleteAsync(request.Id, CancellationToken.None);
  }
}
