using L2.Application.Models;
using L2.Application.UseCases.Auth.Commands.Login;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Auth;

public class LoginHandlerTests {
  [Fact]
  public async Task Handle_ReturnsTokensFromAuthService() {
    var authService = new StubAuthService();
    var handler = new LoginHandler(authService);
    var request = new LoginCommand(new LoginRequest("bidder@example.com", "Password1"), UserRole.Bidder);

    var result = await handler.Handle(
      request,
      TestContext.Current.CancellationToken
    );

    Assert.Equal(authService.LoginResult, result.Tokens);
    Assert.Equal(("bidder@example.com", "Password1", UserRole.Bidder), authService.LoginInput);
  }
}
