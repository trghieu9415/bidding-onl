using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using L2.Application.Models;
using L3.Infrastructure.Options;
using L3.Infrastructure.Services;
using Xunit;

namespace Tests.Integration.L3.Infrastructure.Services;

public class JwtServiceTests {
  private readonly JwtSettings _settings = new() {
    Secret = "super-secret-key-with-at-least-32-chars",
    Issuer = "tests",
    Audience = "tests-client",
    AccessExpiration = 15,
    RefreshExpiration = 60
  };

  [Fact]
  public void ValidateToken_returns_null_for_empty_token() {
    var service = new JwtService(_settings);

    var principal = service.ValidateToken(string.Empty);

    Assert.Null(principal);
  }

  [Fact]
  public void GenerateAccessToken_creates_valid_token_with_expected_claims() {
    var service = new JwtService(_settings);
    var user = CreateUser();

    var token = service.GenerateAccessToken(user);
    var principal = service.ValidateToken(token.Token);

    Assert.NotNull(principal);
    Assert.Equal(user.Id.ToString(), principal!.FindFirstValue(ClaimTypes.NameIdentifier));
    Assert.Equal(user.FullName, principal.FindFirstValue(ClaimTypes.Name));
    Assert.Equal(user.Email, principal.FindFirstValue(ClaimTypes.Email));
    Assert.Equal(user.Role.ToString(), principal.FindFirstValue(ClaimTypes.Role));
    Assert.Equal(user.SecurityStamp, principal.FindFirstValue("security_stamp"));
    Assert.Equal("access", principal.FindFirstValue("token_type"));
    Assert.True(token.ExpiredAt > DateTime.UtcNow);
  }

  [Fact]
  public void GenerateRefreshToken_creates_valid_token_with_refresh_claim() {
    var service = new JwtService(_settings);
    var user = CreateUser();

    var token = service.GenerateRefreshToken(user);
    var principal = service.ValidateToken(token.Token);

    Assert.NotNull(principal);
    Assert.Equal(user.Id.ToString(), principal!.FindFirstValue(ClaimTypes.NameIdentifier));
    Assert.Equal(user.SecurityStamp, principal.FindFirstValue("security_stamp"));
    Assert.Equal("refresh", principal.FindFirstValue("token_type"));
    Assert.NotNull(principal.FindFirstValue(JwtRegisteredClaimNames.Jti));
  }

  [Fact]
  public void ValidateToken_returns_null_when_issuer_does_not_match() {
    var service = new JwtService(_settings);
    var otherService = new JwtService(new JwtSettings {
      Secret = _settings.Secret,
      Issuer = "other",
      Audience = _settings.Audience,
      AccessExpiration = _settings.AccessExpiration,
      RefreshExpiration = _settings.RefreshExpiration
    });

    var token = otherService.GenerateAccessToken(CreateUser());

    var principal = service.ValidateToken(token.Token);

    Assert.Null(principal);
  }

  private static User CreateUser() {
    return new User {
      Id = Guid.NewGuid(),
      FullName = "Integration Tester",
      Email = "tester@example.com",
      Role = UserRole.Bidder,
      SecurityStamp = "stamp-123"
    };
  }
}
