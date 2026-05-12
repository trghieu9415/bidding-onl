using FluentAssertions;
using L2.Application.Ports.Security;
using L2.Application.UseCases.Bidders.Commands.UnlockBidder;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Bidders.Commands;

public class UnlockBidderHandlerTests {
  private readonly IUserService _userService = Substitute.For<IUserService>();
  private readonly UnlockBidderHandler _sut;

  public UnlockBidderHandlerTests() {
    _sut = new UnlockBidderHandler(_userService);
  }

  [Fact]
  public async Task Handle_Should_UnlockBidder_And_ReturnTrue() {
    var request = new UnlockBidderCommand(Guid.NewGuid());

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    await _userService.Received(1).UnlockAsync(request.Id, CancellationToken.None);
  }
}
