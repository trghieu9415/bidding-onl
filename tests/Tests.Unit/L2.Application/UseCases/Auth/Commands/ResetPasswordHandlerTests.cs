using FluentAssertions;
using L2.Application.Ports.Security;
using L2.Application.UseCases.Auth.Commands.ResetPassword;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth.Commands;

public class ResetPasswordHandlerTests {
  private readonly IAuthService _authService = Substitute.For<IAuthService>();
  private readonly ResetPasswordHandler _sut;

  public ResetPasswordHandlerTests() {
    _sut = new ResetPasswordHandler(_authService);
  }

  [Fact]
  public async Task Handle_Should_ResetPassword_And_ReturnTrue() {
    var request = new ResetPasswordCommand("user@example.com", "token", "Password123");

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    await _authService.Received(1)
      .ResetPasswordAsync(request.Email, request.Token, request.NewPassword, CancellationToken.None);
  }
}
