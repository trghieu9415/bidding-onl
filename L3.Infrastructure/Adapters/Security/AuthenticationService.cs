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
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace L3.Infrastructure.Adapters.Security;

public class AuthenticationService(
  UserManager<AppUser> userManager,
  IJwtService jwtService,
  IConfiguration config,
  IEmailService emailService,
  ICacheStorage cache
) : IAuthentication {
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
    return new AuthTokens(jwtService.GenerateAccessToken(userModel), jwtService.GenerateRefreshToken(userModel));
  }

  public async Task RequestPasswordAsync(string email, CancellationToken ct) {
    var user = await userManager.FindByEmailAsync(email);
    if (user == null || user.IsDeleted || user.Role != UserRole.Bidder) {
      return;
    }

    var tokenModel = jwtService.GenerateRequestToken(ToUserModel(user));
    await emailService.SendResetPasswordEmailAsync(user.Email!, tokenModel.Token, ct);
  }


  public async Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct) {
    var user = await userManager.FindByEmailAsync(email);
    if (user == null || user.IsDeleted) {
      throw new AppException("Thông tin không hợp lệ", 404);
    }

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(config["Jwt:Secret"]!);

    try {
      tokenHandler.ValidateToken(token, new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = config["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero
      }, out var validatedToken);

      var jwtToken = (JwtSecurityToken)validatedToken;
      var emailFromToken = jwtToken.Claims.First(x => x.Type == ClaimTypes.Email).Value;
      var typeFromToken = jwtToken.Claims.First(x => x.Type == "token_type").Value;

      if (emailFromToken != email || typeFromToken != "reset_password") {
        throw new AppException("Mã xác nhận không hợp lệ hoặc không đúng loại");
      }
    } catch {
      throw new AppException("Mã xác nhận đã hết hạn hoặc không hợp lệ", 401);
    }

    var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
    var result = await userManager.ResetPasswordAsync(user, resetToken, newPassword);

    if (!result.Succeeded) {
      throw new AppException(result.Errors.First().Description);
    }
  }

  public async Task<AuthTokens> RefreshAsync(string refreshToken, CancellationToken ct) {
    var isBlacklisted = await cache.GetAsync<bool>($"blacklist:{refreshToken}", ct);
    if (isBlacklisted) {
      throw new AppException("Phiên đăng nhập đã bị vô hiệu hóa", 401);
    }

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(config["Jwt:Secret"]!);

    try {
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
      var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

      var user = await userManager.FindByIdAsync(userId);
      if (user == null || user.IsDeleted) {
        throw new AppException("Tài khoản không hợp lệ", 401);
      }

      var remainingTime = jwtToken.ValidTo - DateTime.UtcNow;
      if (remainingTime.TotalSeconds > 0) {
        await cache.SetAsync($"blacklist:{refreshToken}", true, remainingTime, ct);
      }

      var userModel = ToUserModel(user);
      return new AuthTokens(jwtService.GenerateAccessToken(userModel), jwtService.GenerateRefreshToken(userModel));
    } catch {
      throw new AppException("Phiên đăng nhập không hợp lệ hoặc đã hết hạn", 401);
    }
  }

  public async Task LogoutAsync(string refreshToken, CancellationToken ct) {
    if (string.IsNullOrEmpty(refreshToken)) {
      return;
    }

    var tokenHandler = new JwtSecurityTokenHandler();
    try {
      var jwtToken = tokenHandler.ReadJwtToken(refreshToken);
      var remainingTime = jwtToken.ValidTo - DateTime.UtcNow;

      if (remainingTime.TotalSeconds > 0) {
        await cache.SetAsync($"blacklist:{refreshToken}", true, remainingTime, ct);
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
      Role = u.Role
    };
  }
}
