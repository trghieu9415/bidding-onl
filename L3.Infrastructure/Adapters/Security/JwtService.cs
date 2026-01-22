using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using L2.Application.Models;
using L2.Application.Ports.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace L3.Infrastructure.Adapters.Security;

public class JwtService(IConfiguration config) : IJwtService {
  public TokenModel GenerateAccessToken(User user) {
    var claims = new List<Claim> {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new(ClaimTypes.Email, user.Email),
      new(ClaimTypes.Name, user.FullName),
      new(ClaimTypes.Role, nameof(user.Role))
    };
    return GenerateToken(claims, int.Parse(config["Jwt:AccessExpiration"] ?? "60"));
  }

  public TokenModel GenerateRefreshToken(User user) {
    var claims = new List<Claim> {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new("token_type", "refresh")
    };
    return GenerateToken(claims, int.Parse(config["Jwt:RefreshExpiration"] ?? "1440"));
  }

  public TokenModel GenerateRequestToken(User user) {
    var claims = new List<Claim> {
      new(ClaimTypes.Email, user.Email),
      new("token_type", "reset_password")
    };
    return GenerateToken(claims, 15);
  }

  private TokenModel GenerateToken(IEnumerable<Claim> claims, int expirationMinutes) {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Secret"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var expiry = DateTime.Now.AddMinutes(expirationMinutes);

    var token = new JwtSecurityToken(
      config["Jwt:Issuer"],
      config["Jwt:Audience"],
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