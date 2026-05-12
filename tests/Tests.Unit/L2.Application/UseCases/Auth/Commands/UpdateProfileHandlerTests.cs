using FluentAssertions;
using L2.Application.Exceptions;
using L2.Application.Models;
using L2.Application.Ports.Security;
using L2.Application.UseCases.Auth.Commands.UpdateProfile;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth.Commands;

public class UpdateProfileHandlerTests {
  private readonly IUserService _userService = Substitute.For<IUserService>();
  private readonly UpdateProfileHandler _sut;

  public UpdateProfileHandlerTests() {
    _sut = new UpdateProfileHandler(_userService);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_UserNotFound() {
    var userId = Guid.NewGuid();
    var request = new UpdateProfileCommand(userId, UserRole.Bidder, new UpdateProfileRequest("Name", "0912", "https://example.com"));

    _userService.GetByIdAsync(userId, request.Role, CancellationToken.None)
      .Returns((User?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.Message.Should().Be($"Không tìm thấy người dùng - Id:{userId}");
  }

  [Fact]
  public async Task Handle_Should_UpdateProfile_And_ReturnTrue() {
    var user = UseCaseTestData.CreateUser(role: UserRole.Bidder);
    var request = new UpdateProfileCommand(
      user.Id,
      UserRole.Bidder,
      new UpdateProfileRequest("New Name", "0987", "https://new.example.com")
    );

    _userService.GetByIdAsync(user.Id, request.Role, CancellationToken.None)
      .Returns(user);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    await _userService.Received(1).UpdateAsync(
      Arg.Is<User>(x =>
        x.Id == user.Id &&
        x.FullName == request.Data.FullName &&
        x.PhoneNumber == request.Data.PhoneNumber &&
        x.Url == request.Data.Url
      ),
      CancellationToken.None
    );
  }
}
