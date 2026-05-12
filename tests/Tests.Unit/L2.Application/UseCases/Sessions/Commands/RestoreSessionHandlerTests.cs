using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.Repositories;
using L2.Application.UseCases.Sessions.Commands.RestoreSession;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Sessions.Commands;

public class RestoreSessionHandlerTests {
  private readonly IRepository<AuctionSession> _repository = Substitute.For<IRepository<AuctionSession>>();
  private readonly RestoreSessionHandler _sut;

  public RestoreSessionHandlerTests() {
    _sut = new RestoreSessionHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_RestoreSession_And_ReturnTrue() {
    var request = new RestoreSessionCommand(Guid.NewGuid());

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    await _repository.Received(1).RestoreAsync(request.Id, CancellationToken.None);
  }
}
