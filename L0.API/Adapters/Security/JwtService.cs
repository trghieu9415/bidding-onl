using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using L2.Application.Models;
using L2.Application.Ports.Security;
using L3.Infrastructure.Options;
using Microsoft.IdentityModel.Tokens;

namespace L0.API.Adapters.Security;

public class JwtService(JwtOptions jwtOptions) : IJwtService {
  public TokenModel GenerateAccessToken(User user) {
    var claims = new List<Claim> {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new(ClaimTypes.Email, user.Email),
      new(ClaimTypes.Name, user.FullName),
      new(ClaimTypes.Role, nameof(user.Role)),
      new("security_stamp", user.SecurityStamp ?? ""),
      new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
    return GenerateToken(claims, jwtOptions.AccessExpiration);
  }

  public TokenModel GenerateRefreshToken(User user) {
    var claims = new List<Claim> {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new("security_stamp", user.SecurityStamp ?? ""),
      new("token_type", "refresh"),
      new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
    return GenerateToken(claims, jwtOptions.RefreshExpiration);
  }

  private TokenModel GenerateToken(IEnumerable<Claim> claims, int expirationMinutes) {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var expiry = DateTime.UtcNow.AddMinutes(expirationMinutes);

    var token = new JwtSecurityToken(
      jwtOptions.Issuer,
      jwtOptions.Audience,
      claims,
      expires: expiry,
      signingCredentials: creds
    );

    return new TokenModel {
      Token = new JwtSecurityTokenHandler().WriteToken(token),
      ExpiredAt = expiry
    };
  }
}
