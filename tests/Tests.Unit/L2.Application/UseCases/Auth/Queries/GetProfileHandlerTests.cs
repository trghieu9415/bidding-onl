using FluentAssertions;
using L2.Application.Exceptions;
using L2.Application.Models;
using L2.Application.Ports.Security;
using L2.Application.UseCases.Auth.Queries.GetProfile;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth.Queries;

public class GetProfileHandlerTests {
  private readonly IUserService _userService = Substitute.For<IUserService>();
  private readonly GetProfileHandler _sut;

  public GetProfileHandlerTests() {
    _sut = new GetProfileHandler(_userService);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_UserNotFound() {
    var request = new GetProfileQuery(Guid.NewGuid(), UserRole.Admin);

    _userService.GetByIdAsync(request.Id, request.Role, CancellationToken.None)
      .Returns((User?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Người dùng không tồn tại");
  }

  [Fact]
  public async Task Handle_Should_ReturnUser_When_UserExists() {
    var user = UseCaseTestData.CreateUser(role: UserRole.Admin);
    var request = new GetProfileQuery(user.Id, UserRole.Admin);

    _userService.GetByIdAsync(request.Id, request.Role, CancellationToken.None)
      .Returns(user);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.User.Should().Be(user);
  }
}
