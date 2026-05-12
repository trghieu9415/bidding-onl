using FluentAssertions;
using L2.Application.Models;
using L2.Application.Ports.Security;
using L2.Application.UseCases.Auth.Commands.Login;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth.Commands;

public class LoginHandlerTests {
  private readonly IAuthService _authService = Substitute.For<IAuthService>();
  private readonly LoginHandler _sut;

  public LoginHandlerTests() {
    _sut = new LoginHandler(_authService);
  }

  [Fact]
  public async Task Handle_Should_Login_And_ReturnTokens() {
    var tokens = UseCaseTestData.CreateAuthTokens();
    var request = new LoginCommand(new LoginRequest("user@example.com", "Password123"), UserRole.Bidder);

    _authService.LoginAsync(request.Data.Email, request.Data.Password, request.Role, CancellationToken.None)
      .Returns(tokens);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Tokens.Should().Be(tokens);
  }
}
