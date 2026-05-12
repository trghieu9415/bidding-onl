using FluentAssertions;
using L2.Application.Ports.Security;
using L2.Application.UseCases.Auth.Commands.Logout;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth.Commands;

public class LogoutHandlerTests {
  private readonly IAuthService _authService = Substitute.For<IAuthService>();
  private readonly LogoutHandler _sut;

  public LogoutHandlerTests() {
    _sut = new LogoutHandler(_authService);
  }

  [Fact]
  public async Task Handle_Should_Logout_And_ReturnTrue() {
    var request = new LogoutCommand("refresh-token", true);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    await _authService.Received(1).LogoutAsync(request.RefreshToken, request.AllDevices, CancellationToken.None);
  }
}
