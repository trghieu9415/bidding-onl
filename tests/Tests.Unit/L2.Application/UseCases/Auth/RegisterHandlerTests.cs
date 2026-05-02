using L2.Application.Models;
using L2.Application.UseCases.Auth.Commands.Register;
using Tests.Unit.L2.Application.UseCases.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth;

public class RegisterHandlerTests {
  [Fact]
  public async Task Handle_MapsCommandIntoUserAndReturnsTokens() {
    var authService = new StubAuthService();
    var handler = new RegisterHandler(authService);

    var result =
      await handler.Handle(
        new RegisterCommand("bidder@example.com", "Bidder Name", "Password1", "0123456789"),
        TestContext.Current.CancellationToken
      );

    Assert.Equal(authService.RegisterResult, result.Tokens);
    var user = Assert.IsType<User>(authService.RegisteredUser);
    Assert.Equal("bidder@example.com", user.Email);
    Assert.Equal("Bidder Name", user.FullName);
    Assert.Equal("0123456789", user.PhoneNumber);
    Assert.Equal("Password1", authService.RegisteredPassword);
  }
}
