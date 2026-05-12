using FluentAssertions;
using L2.Application.Ports.Security;
using L2.Application.UseCases.Bidders.Commands.LockBidder;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Bidders.Commands;

public class LockBidderHandlerTests {
  private readonly IUserService _userService = Substitute.For<IUserService>();
  private readonly LockBidderHandler _sut;

  public LockBidderHandlerTests() {
    _sut = new LockBidderHandler(_userService);
  }

  [Fact]
  public async Task Handle_Should_LockBidder_And_ReturnTrue() {
    var request = new LockBidderCommand(Guid.NewGuid());

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    await _userService.Received(1).LockAsync(request.Id, CancellationToken.None);
  }
}
