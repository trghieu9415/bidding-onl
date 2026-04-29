using System.Security.Claims;
using L2.Application.Models;

namespace L3.Infrastructure.Services.Abstractions;

public interface IJwtService {
  TokenModel GenerateAccessToken(User user);
  TokenModel GenerateRefreshToken(User user);
  ClaimsPrincipal? ValidateToken(string token);
}
