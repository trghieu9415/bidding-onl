using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using L2.Application.Models;
using L3.Infrastructure.Options;
using L3.Infrastructure.Services.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace L3.Infrastructure.Services;

public class JwtService(JwtSettings jwtSettings) : IJwtService {
  public ClaimsPrincipal? ValidateToken(string token) {
    if (string.IsNullOrEmpty(token)) {
      return null;
    }

    var tokenHandler = new JwtSecurityTokenHandler();
    try {
      return tokenHandler.ValidateToken(token, GetValidationParameters(jwtSettings), out _);
    } catch {
      return null;
    }
  }

  public TokenModel GenerateAccessToken(User user) {
    var claims = new List<Claim> {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new(ClaimTypes.Name, user.FullName ?? ""),
      new(ClaimTypes.Email, user.Email),
      new(ClaimTypes.Role, user.Role.ToString()),
      new("security_stamp", user.SecurityStamp ?? ""),
      new("token_type", "access"),
      new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
    return GenerateToken(claims, jwtSettings.AccessExpiration);
  }

  public TokenModel GenerateRefreshToken(User user) {
    var claims = new List<Claim> {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new("security_stamp", user.SecurityStamp ?? ""),
      new("token_type", "refresh"),
      new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
    return GenerateToken(claims, jwtSettings.RefreshExpiration);
  }

  private TokenModel GenerateToken(IEnumerable<Claim> claims, int expirationMinutes) {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
    var expiry = DateTime.UtcNow.AddMinutes(expirationMinutes);
    var token = new JwtSecurityToken(
      jwtSettings.Issuer,
      jwtSettings.Audience,
      claims,
      expires: expiry,
      signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
    );
    return new TokenModel {
      Token = new JwtSecurityTokenHandler().WriteToken(token),
      ExpiredAt = expiry
    };
  }

  public static TokenValidationParameters GetValidationParameters(JwtSettings jwtSettings) {
    return new TokenValidationParameters {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
      ValidateIssuer = true,
      ValidIssuer = jwtSettings.Issuer,
      ValidateAudience = true,
      ValidAudience = jwtSettings.Audience,
      ValidateLifetime = true,
      ClockSkew = TimeSpan.Zero
    };
  }
}
