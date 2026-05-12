using FluentAssertions;
using L2.Application.Ports.Security;
using L2.Application.UseCases.Auth.Commands.ChangePassword;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth.Commands;

public class ChangePasswordHandlerTests {
  private readonly IAuthService _authService = Substitute.For<IAuthService>();
  private readonly ChangePasswordHandler _sut;

  public ChangePasswordHandlerTests() {
    _sut = new ChangePasswordHandler(_authService);
  }

  [Fact]
  public async Task Handle_Should_ChangePassword_And_ReturnTrue() {
    var userId = Guid.NewGuid();
    var request = new ChangePasswordCommand(userId, new ChangePasswordRequest("Old12345", "New12345"));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    await _authService.Received(1)
      .ChangePasswordAsync(userId, request.Data.OldPassword, request.Data.NewPassword, CancellationToken.None);
  }
}
