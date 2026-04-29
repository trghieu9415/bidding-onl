using L2.Application.Models;

namespace L2.Application.Ports.Security;

public interface IAuthService {
  Task<AuthTokens> LoginAsync(string email, string password, UserRole role, CancellationToken ct = default);
  Task<AuthTokens> RegisterAsync(User user, string password, CancellationToken ct = default);
  Task<AuthTokens> RefreshAsync(string refreshToken, CancellationToken ct = default);
  Task LogoutAsync(string refreshToken, bool revokeAll, CancellationToken ct = default);

  Task<AuthTokens> ChangePasswordAsync(
    Guid userId, string oldPassword, string newPassword, CancellationToken ct = default
  );

  Task RequestPasswordAsync(string email, CancellationToken ct = default);
  Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default);

  Task<bool> ValidateSecurityStampAsync(Guid userId, string tokenSecurityStamp, CancellationToken ct = default);
}
