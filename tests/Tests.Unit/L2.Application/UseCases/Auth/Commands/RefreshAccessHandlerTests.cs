using FluentAssertions;
using L2.Application.Ports.Security;
using L2.Application.UseCases.Auth.Commands.RefreshAccess;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth.Commands;

public class RefreshAccessHandlerTests {
  private readonly IAuthService _authService = Substitute.For<IAuthService>();
  private readonly RefreshAccessHandler _sut;

  public RefreshAccessHandlerTests() {
    _sut = new RefreshAccessHandler(_authService);
  }

  [Fact]
  public async Task Handle_Should_Refresh_And_ReturnTokens() {
    var tokens = UseCaseTestData.CreateAuthTokens();
    var request = new RefreshAccessCommand("refresh-token");

    _authService.RefreshAsync(request.RefreshToken, CancellationToken.None)
      .Returns(tokens);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Tokens.Should().Be(tokens);
  }
}
