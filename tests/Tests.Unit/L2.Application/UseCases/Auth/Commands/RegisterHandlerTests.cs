using FluentAssertions;
using L2.Application.Models;
using L2.Application.Ports.Security;
using L2.Application.UseCases.Auth.Commands.Register;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth.Commands;

public class RegisterHandlerTests {
  private readonly IAuthService _authService = Substitute.For<IAuthService>();
  private readonly RegisterHandler _sut;

  public RegisterHandlerTests() {
    _sut = new RegisterHandler(_authService);
  }

  [Fact]
  public async Task Handle_Should_Map_User_And_ReturnTokens() {
    var tokens = UseCaseTestData.CreateAuthTokens();
    var request = new RegisterCommand("user@example.com", "Nguyen Van A", "Password123", "0912345678");

    _authService.RegisterAsync(
        Arg.Is<User>(x =>
          x.Email == request.Email &&
          x.FullName == request.FullName &&
          x.PhoneNumber == request.PhoneNumber
        ),
        request.Password,
        CancellationToken.None
      )
      .Returns(tokens);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Tokens.Should().Be(tokens);
  }
}
