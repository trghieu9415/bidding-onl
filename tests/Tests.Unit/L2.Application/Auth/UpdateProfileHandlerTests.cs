using L2.Application.Exceptions;
using L2.Application.Models;
using L2.Application.UseCases.Auth.Commands.UpdateProfile;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Auth;

public class UpdateProfileHandlerTests {
  [Fact]
  public async Task Handle_WhenUserMissing_ThrowsWorkflowException() {
    var handler = new UpdateProfileHandler(new StubUserService());

    var exception = await Assert.ThrowsAsync<WorkflowException>(async () =>
      await handler.Handle(
        new UpdateProfileCommand(Guid.NewGuid(), UserRole.Bidder, new UpdateProfileRequest("Name", "0123", "url")),
        TestContext.Current.CancellationToken));

    Assert.StartsWith("Không tìm thấy người dùng - Id:", exception.Message);
  }

  [Fact]
  public async Task Handle_WhenUserFound_UpdatesProjectedFields() {
    var originalUser = new User {
      Id = Guid.NewGuid(),
      FullName = "Old Name",
      Email = "bidder@example.com",
      PhoneNumber = "0123",
      Url = "old-url",
      Role = UserRole.Bidder
    };
    var userService = new StubUserService { UserByIdResult = originalUser };
    var handler = new UpdateProfileHandler(userService);
    var request = new UpdateProfileCommand(
      originalUser.Id,
      UserRole.Bidder,
      new UpdateProfileRequest("New Name", "0999", "new-url")
    );

    var result = await handler.Handle(request, default);

    Assert.True(result);
    var updatedUser = Assert.IsType<User>(userService.UpdatedUser);
    Assert.Equal(originalUser.Id, updatedUser.Id);
    Assert.Equal("New Name", updatedUser.FullName);
    Assert.Equal("0999", updatedUser.PhoneNumber);
    Assert.Equal("new-url", updatedUser.Url);
    Assert.Equal(originalUser.Email, updatedUser.Email);
  }
}
