using L2.Application.UseCases.Auth.Commands.ChangePassword;
using Tests.Unit.L2.Application.UseCases.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth;

public class ChangePasswordHandlerTests {
  [Fact]
  public async Task Handle_CallsAuthServiceAndReturnsTrue() {
    var authService = new StubAuthService();
    var handler = new ChangePasswordHandler(authService);
    var userId = Guid.NewGuid();

    var result = await handler.Handle(
      new ChangePasswordCommand(userId, new ChangePasswordRequest("OldPass1", "NewPass1")),
      TestContext.Current.CancellationToken
    );

    Assert.True(result);
    Assert.Equal((userId, "OldPass1", "NewPass1"), authService.ChangePasswordInput);
  }
}
