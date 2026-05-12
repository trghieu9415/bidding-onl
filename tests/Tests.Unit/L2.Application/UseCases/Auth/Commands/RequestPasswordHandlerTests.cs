using FluentAssertions;
using L2.Application.Ports.Security;
using L2.Application.UseCases.Auth.Commands.RequestPassword;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth.Commands;

public class RequestPasswordHandlerTests {
  private readonly IAuthService _authService = Substitute.For<IAuthService>();
  private readonly RequestPasswordHandler _sut;

  public RequestPasswordHandlerTests() {
    _sut = new RequestPasswordHandler(_authService);
  }

  [Fact]
  public async Task Handle_Should_RequestPassword_And_ReturnTrue() {
    var request = new RequestPasswordCommand("user@example.com");

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    await _authService.Received(1).RequestPasswordAsync(request.Email, CancellationToken.None);
  }
}
