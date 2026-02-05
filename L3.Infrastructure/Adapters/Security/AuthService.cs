using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using L2.Application.Exceptions;
using L2.Application.Models;
using L2.Application.Ports.Notification;
using L2.Application.Ports.Security;
using L2.Application.Ports.Storage;
using L3.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace L3.Infrastructure.Adapters.Security;

public class AuthService(
  UserManager<AppUser> userManager,
  IJwtService jwtService,
  IConfiguration config,
  IEmailService emailService,
  ICacheStorage cache
) : IAuthService {
  public async Task<AuthTokens> LoginUserAsync(string email, string password, CancellationToken ct) {
    return await AuthenticateAsync(email, password, UserRole.Bidder);
  }

  public async Task<AuthTokens> LoginAdminAsync(string email, string password, CancellationToken ct) {
    return await AuthenticateAsync(email, password, UserRole.Admin);
  }

  public async Task<AuthTokens> RegisterAsync(User user, string password, CancellationToken ct) {
    var existingUser = await userManager.FindByEmailAsync(user.Email);
    if (existingUser != null) {
      throw new AppException("Email đã tồn tại trong hệ thống");
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
      throw new AppException(result.Errors.First().Description);
    }

    var userModel = ToUserModel(appUser);
    return new AuthTokens(jwtService.GenerateAccessToken(userModel), jwtService.GenerateRefreshToken(userModel));
  }

  public async Task<AuthTokens> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword,
    CancellationToken ct) {
    var user = await userManager.FindByIdAsync(userId.ToString());
    if (user == null || user.IsDeleted) {
      throw new AppException("Người dùng không tồn tại hoặc đã bị xóa", 404);
    }

    var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
    if (!result.Succeeded) {
      var error = result.Errors.FirstOrDefault()?.Description ?? "Mật khẩu cũ không đúng";
      throw new AppException(error);
    }

    var userModel = ToUserModel(user);
    return new AuthTokens(
      jwtService.GenerateAccessToken(userModel),
      jwtService.GenerateRefreshToken(userModel)
    );
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
      throw new AppException("Thông tin không hợp lệ", 404);
    }

    try {
      var decodedBytes = WebEncoders.Base64UrlDecode(token);
      var originalToken = Encoding.UTF8.GetString(decodedBytes);
      var result = await userManager.ResetPasswordAsync(user, originalToken, newPassword);
      if (!result.Succeeded) {
        var errorMsg = result.Errors.FirstOrDefault()?.Description;
        Console.WriteLine(errorMsg);
        throw new AppException("Đổi mật khẩu thất bại");
      }

      await userManager.UpdateSecurityStampAsync(user);
    } catch {
      throw new AppException("Mã xác nhận đã hết hạn hoặc không hợp lệ", 401);
    }
  }

  public async Task<AuthTokens> RefreshAsync(string refreshToken, CancellationToken ct) {
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(config["Jwt:Secret"]!);

    tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(key),
      ValidateIssuer = true,
      ValidIssuer = config["Jwt:Issuer"],
      ValidateAudience = true,
      ValidAudience = config["Jwt:Audience"],
      ClockSkew = TimeSpan.Zero
    }, out var validatedToken);

    var jwtToken = (JwtSecurityToken)validatedToken;

    var tokenType = jwtToken.Claims.FirstOrDefault(x => x.Type == "token_type")?.Value;
    if (tokenType != "refresh") {
      throw new AppException("Token không hợp lệ");
    }

    var isBlacklisted = await cache.IsBlacklistedAsync(jwtToken.Id, ct);
    if (isBlacklisted) {
      throw new AppException("Phiên đăng nhập đã bị vô hiệu hóa", 401);
    }

    var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

    var user = await userManager.FindByIdAsync(userId);
    if (user == null || user.IsDeleted) {
      throw new AppException("Tài khoản không hợp lệ", 401);
    }

    if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow) {
      throw new AppException("Tài khoản đã bị khóa", 403);
    }

    var tokenStamp = jwtToken.Claims.FirstOrDefault(x => x.Type == "security_stamp")?.Value;
    Console.WriteLine(tokenStamp);
    Console.WriteLine(user.SecurityStamp);
    if (tokenStamp != user.SecurityStamp) {
      throw new AppException("Thông tin bảo mật đã thay đổi, vui lòng đăng nhập lại", 401);
    }

    await cache.BlacklistAsync(jwtToken.Id, jwtToken.ValidTo - DateTime.UtcNow, ct);

    var userModel = ToUserModel(user);
    return new AuthTokens(
      jwtService.GenerateAccessToken(userModel),
      jwtService.GenerateRefreshToken(userModel)
    );
  }

  public async Task LogoutAsync(string refreshToken, bool revokeAll, CancellationToken ct) {
    if (string.IsNullOrEmpty(refreshToken)) {
      return;
    }

    var tokenHandler = new JwtSecurityTokenHandler();
    try {
      var jwtToken = tokenHandler.ReadJwtToken(refreshToken);

      if (revokeAll) {
        var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        var user = await userManager.FindByIdAsync(userId);
        if (user != null) {
          await userManager.UpdateSecurityStampAsync(user);
        }
      } else {
        var remainingTime = jwtToken.ValidTo - DateTime.UtcNow;

        if (remainingTime.TotalSeconds > 0) {
          await cache.BlacklistAsync(jwtToken.Id, remainingTime, ct);
        }
      }
    } catch {
      // ignored
    }
  }

  // NOTE: ========== [Helper methods] ==========
  private async Task<AuthTokens> AuthenticateAsync(string email, string password, UserRole role) {
    var user = await userManager.FindByEmailAsync(email);

    if (user == null || user.IsDeleted || user.Role != role || !await userManager.CheckPasswordAsync(user, password)) {
      throw new AppException("Thông tin đăng nhập không chính xác", 401);
    }

    if (await userManager.IsLockedOutAsync(user)) {
      throw new AppException("Tài khoản đang bị khóa. Vui lòng liên hệ Admin.", 403);
    }

    var userModel = ToUserModel(user);
    return new AuthTokens(jwtService.GenerateAccessToken(userModel), jwtService.GenerateRefreshToken(userModel));
  }

  private static User ToUserModel(AppUser u) {
    return new User {
      Id = u.Id,
      Email = u.Email!,
      FullName = u.FullName,
      PhoneNumber = u.PhoneNumber,
      Url = u.Url,
      IsActive = u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow,
      Role = u.Role,
      SecurityStamp = u.SecurityStamp
    };
  }
}
