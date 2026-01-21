using L2.Application.Models;

namespace L2.Application.Ports.Security;

public interface IJwtService {
  TokenModel GenerateAccessToken(User user);
  TokenModel GenerateRefreshToken(User user);
  TokenModel GenerateRequestToken(User user);
}
