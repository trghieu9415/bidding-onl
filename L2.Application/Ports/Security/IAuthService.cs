using L2.Application.Models;

namespace L2.Application.Ports.Security;

public interface IAuthService {
  Task<AuthTokens> LoginUserAsync(string email, string password, CancellationToken ct);
  Task<AuthTokens> LoginAdminAsync(string email, string password, CancellationToken ct);
  Task<AuthTokens> RegisterAsync(User user, string password, CancellationToken ct);
  Task<AuthTokens> RefreshAsync(string refreshToken, CancellationToken ct);
  Task LogoutAsync(string refreshToken, bool revokeAll, CancellationToken ct);

  Task<AuthTokens> ChangePasswordAsync(
    Guid userId, string oldPassword, string newPassword, CancellationToken ct
  );

  Task RequestPasswordAsync(string email, CancellationToken ct);
  Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct);
}
