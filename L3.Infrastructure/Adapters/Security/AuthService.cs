using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using L2.Application.Exceptions;
using L2.Application.Models;
using L2.Application.Ports.Security;
using L3.Infrastructure.Identity;
using L3.Infrastructure.Services.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace L3.Infrastructure.Adapters.Security;

public class AuthService(
  UserManager<AppUser> userManager,
  IJwtService jwtService,
  IEmailService emailService,
  ICacheService cache
) : IAuthService {
  public async Task<AuthTokens> LoginAsync(string email, string password, UserRole role, CancellationToken ct) {
    var user = await userManager.FindByEmailAsync(email);

    if (
      user == null ||
      user.IsDeleted ||
      user.Role != role ||
      !await userManager.CheckPasswordAsync(user, password)
    ) {
      throw new WorkflowException("Thông tin đăng nhập không chính xác", 401);
    }

    if (await userManager.IsLockedOutAsync(user)) {
      throw new WorkflowException("Tài khoản đang bị khóa. Vui lòng liên hệ Admin.", 403);
    }

    await cache.SyncSecurityStampAsync(user.Id, user.SecurityStamp!, ct);
    return CreateAuthTokens(user);
  }

  public async Task<AuthTokens> RegisterAsync(User user, string password, CancellationToken ct) {
    var existingUser = await userManager.FindByEmailAsync(user.Email);
    if (existingUser != null) {
      throw new WorkflowException("Email đã tồn tại trong hệ thống");
    }

    var appUser = new AppUser {
      Id = Guid.NewGuid(),
      Email = user.Email,
      UserName = user.Email,
      FullName = user.FullName,
      Role = UserRole.Bidder,
      PhoneNumber = user.PhoneNumber
    };

    var result = await userManager.CreateAsync(appUser, password);
    if (!result.Succeeded) {
      throw new WorkflowException(result.Errors.First().Description);
    }

    await cache.SyncSecurityStampAsync(appUser.Id, appUser.SecurityStamp!, ct);
    return CreateAuthTokens(appUser);
  }

  public async Task<AuthTokens> RefreshAsync(string refreshToken, CancellationToken ct) {
    var principal = jwtService.ValidateToken(refreshToken);
    if (principal?.FindFirst("token_type")?.Value != "refresh") {
      throw new WorkflowException("Token không hợp lệ hoặc đã hết hạn", 401);
    }

    var jti = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
    var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
    var tokenStamp = principal.FindFirstValue("security_stamp");

    if (await cache.IsBlacklistedAsync(jti!, ct)) {
      throw new WorkflowException("Phiên đăng nhập đã bị vô hiệu hóa", 401);
    }

    var user = await userManager.FindByIdAsync(userId!);
    if (user == null || user.IsDeleted || user.SecurityStamp != tokenStamp) {
      throw new WorkflowException("Thông tin bảo mật đã thay đổi hoặc tài khoản không tồn tại", 401);
    }

    if (await userManager.IsLockedOutAsync(user)) {
      throw new WorkflowException("Tài khoản đã bị khóa", 403);
    }

    var expClaim = principal.FindFirstValue(JwtRegisteredClaimNames.Exp);
    if (!long.TryParse(expClaim, out var expUnix)) {
      return CreateAuthTokens(user);
    }

    var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
    var remainingTime = expiresAt - DateTime.UtcNow;
    if (remainingTime.TotalSeconds > 0) {
      await cache.BlacklistAsync(jti!, remainingTime, ct);
    }

    return CreateAuthTokens(user);
  }

  public async Task<bool> ValidateSecurityStampAsync(
    Guid userId,
    string tokenSecurityStamp,
    CancellationToken ct
  ) {
    var cachedStamp = await cache.GetSecurityStampAsync(userId, ct);
    if (cachedStamp != null) {
      return tokenSecurityStamp == cachedStamp;
    }

    var user = await userManager.FindByIdAsync(userId.ToString());
    if (user == null || user.IsDeleted) {
      return false;
    }

    await cache.SyncSecurityStampAsync(userId, user.SecurityStamp!, ct);
    return tokenSecurityStamp == user.SecurityStamp;
  }

  public async Task<AuthTokens> ChangePasswordAsync(
    Guid userId,
    string oldPassword, string newPassword,
    CancellationToken ct
  ) {
    var user = await userManager.FindByIdAsync(userId.ToString());
    if (user == null || user.IsDeleted) {
      throw new WorkflowException("Người dùng không tồn tại", 404);
    }

    var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
    if (!result.Succeeded) {
      throw new WorkflowException(result.Errors.FirstOrDefault()?.Description ?? "Đổi mật khẩu thất bại");
    }

    await UpdateSecurityStampInternal(user, ct);
    return CreateAuthTokens(user);
  }

  public async Task RequestPasswordAsync(string email, CancellationToken ct) {
    var user = await userManager.FindByEmailAsync(email);
    if (user == null || user.IsDeleted || user.Role != UserRole.Bidder) {
      return;
    }

    var token = await userManager.GeneratePasswordResetTokenAsync(user);
    var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
    await emailService.SendResetPasswordEmailAsync(user.Email!, encodedToken, ct);
  }

  public async Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct) {
    var user = await userManager.FindByEmailAsync(email);
    if (user == null || user.IsDeleted) {
      throw new WorkflowException("Thông tin không hợp lệ", 404);
    }

    try {
      var originalToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
      var result = await userManager.ResetPasswordAsync(user, originalToken, newPassword);
      if (!result.Succeeded) {
        throw new WorkflowException("Mã xác nhận không hợp lệ hoặc đã hết hạn");
      }

      await UpdateSecurityStampInternal(user, ct);
    } catch (Exception ex) when (ex is not WorkflowException) {
      throw new WorkflowException("Xử lý yêu cầu thất bại", 401);
    }
  }

  public async Task LogoutAsync(string refreshToken, bool revokeAll, CancellationToken ct) {
    if (string.IsNullOrEmpty(refreshToken)) {
      return;
    }

    try {
      var tokenHandler = new JwtSecurityTokenHandler();
      var jwtToken = tokenHandler.ReadJwtToken(refreshToken);

      if (revokeAll) {
        var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        var user = await userManager.FindByIdAsync(userId);
        if (user != null) {
          await UpdateSecurityStampInternal(user, ct);
        }
      } else {
        var remainingTime = jwtToken.ValidTo - DateTime.UtcNow;
        if (remainingTime.TotalSeconds > 0) {
          await cache.BlacklistAsync(jwtToken.Id, remainingTime, ct);
        }
      }
    } catch {
      /* ignored */
    }
  }

  // NOTE: ========== [Private Helpers] ==========

  private async Task UpdateSecurityStampInternal(AppUser user, CancellationToken ct) {
    var result = await userManager.UpdateSecurityStampAsync(user);
    if (result.Succeeded) {
      await cache.SyncSecurityStampAsync(user.Id, user.SecurityStamp!, ct);
    }
  }

  private AuthTokens CreateAuthTokens(AppUser user) {
    var userModel = ToUserModel(user);
    return new AuthTokens(
      jwtService.GenerateAccessToken(userModel),
      jwtService.GenerateRefreshToken(userModel)
    );
  }

  private static User ToUserModel(AppUser u) {
    return new User {
      Id = u.Id,
      Email = u.Email!,
      FullName = u.FullName,
      PhoneNumber = u.PhoneNumber,
      Url = u.Url,
      Role = u.Role,
      SecurityStamp = u.SecurityStamp,
      IsActive = u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow
    };
  }
}
