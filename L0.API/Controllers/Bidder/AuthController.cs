using L0.API.Response;
using L2.Application.Models;
using L2.Application.UseCases.Auth.Commands.ChangePassword;
using L2.Application.UseCases.Auth.Commands.Login;
using L2.Application.UseCases.Auth.Commands.Logout;
using L2.Application.UseCases.Auth.Commands.RefreshAccess;
using L2.Application.UseCases.Auth.Commands.Register;
using L2.Application.UseCases.Auth.Commands.RequestPassword;
using L2.Application.UseCases.Auth.Commands.ResetPassword;
using L2.Application.UseCases.Auth.Commands.Test;
using L2.Application.UseCases.Auth.Commands.UpdateProfile;
using L2.Application.UseCases.Auth.Queries.GetProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace L0.API.Controllers.Bidder;

public class AuthController : UserController {
  [HttpPost("register")]
  [ProducesSuccess<TokenModel>]
  [AllowAnonymous]
  [EnableRateLimiting("AuthPolicy")]
  public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct) {
    var result = await Mediator.Send(command, ct);
    return ApiResponse.Success(result.Tokens, "Đăng ký tài khoản thành công");
  }

  [HttpPost("login")]
  [ProducesSuccess<TokenModel>]
  [AllowAnonymous]
  [EnableRateLimiting("AuthPolicy")]
  public async Task<IActionResult> Login([FromBody] LoginRequest data, CancellationToken ct) {
    var command = new LoginCommand(data, UserRole.Bidder);
    var result = await Mediator.Send(command, ct);
    var tokens = result.Tokens;

    var cookieOptions = new CookieOptions {
      Expires = tokens.Refresh.ExpiredAt,
      Secure = true,
      SameSite = SameSiteMode.None,
      HttpOnly = true
    };

    Response.Cookies.Append("Refresh", tokens.Refresh.Token, cookieOptions);
    return ApiResponse.Success(tokens.Access, "Đăng nhập thành công");
  }

  [HttpGet("profile")]
  [ProducesSuccess<User>]
  public async Task<IActionResult> GetProfile(CancellationToken ct) {
    var query = new GetProfileQuery(CurrentUser.Id, UserRole.Bidder);
    var result = await Mediator.Send(query, ct);
    return ApiResponse.Success(result.User);
  }

  [HttpPut("profile")]
  [ProducesSuccess<bool>]
  public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command, CancellationToken ct) {
    var result = await Mediator.Send(command, ct);
    return ApiResponse.Success(result, "Cập nhật thông tin cá nhân thành công");
  }

  [HttpPatch("change-password")]
  [ProducesSuccess<bool>]
  public async Task<IActionResult> ChangePassword(
    [FromBody] ChangePasswordRequest req,
    CancellationToken ct
  ) {
    var command = new ChangePasswordCommand(CurrentUser.Id, req);
    var result = await Mediator.Send(command, ct);
    return ApiResponse.Success(result, "Đổi mật khẩu thành công");
  }

  [HttpPost("forgot-password")]
  [ProducesSuccess<bool>]
  [AllowAnonymous]
  [EnableRateLimiting("AuthPolicy")]
  public async Task<IActionResult> ForgotPassword([FromBody] RequestPasswordCommand command, CancellationToken ct) {
    var result = await Mediator.Send(command, ct);
    return ApiResponse.Success(result, "Yêu cầu khôi phục mật khẩu đã được gửi đến Email");
  }

  [HttpPost("reset-password")]
  [ProducesSuccess<bool>]
  [EnableRateLimiting("AuthPolicy")]
  public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken ct) {
    var result = await Mediator.Send(command, ct);
    return ApiResponse.Success(result, "Mật khẩu đã được đặt lại thành công");
  }

  [HttpPost("refresh")]
  [ProducesSuccess<TokenModel>]
  public async Task<IActionResult> Refresh(CancellationToken ct) {
    var command = new RefreshAccessCommand(Request.Cookies["Refresh"] ?? string.Empty);
    var result = await Mediator.Send(command, ct);
    var tokens = result.Tokens;

    var cookieOptions = new CookieOptions {
      Expires = tokens.Refresh.ExpiredAt,
      Secure = true,
      SameSite = SameSiteMode.None,
      HttpOnly = true
    };

    Response.Cookies.Append("Refresh", tokens.Refresh.Token, cookieOptions);
    return ApiResponse.Success(tokens.Access, "Làm mới thành công");
  }

  [HttpPost("logout")]
  [ProducesSuccess<bool>]
  public async Task<IActionResult> Logout([FromQuery] bool allDevices, CancellationToken ct) {
    var refreshToken = Request.Cookies["Refresh"] ?? string.Empty;
    var command = new LogoutCommand(refreshToken, allDevices);
    var result = await Mediator.Send(command, ct);
    Response.Cookies.Delete("Refresh", new CookieOptions {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.None
    });
    return ApiResponse.Success(result, "Đăng xuất thành công");
  }

  [HttpPost("test")]
  [AllowAnonymous]
  [ProducesSuccess<bool>]
  public async Task<IActionResult> Test([FromBody] TestCommand command, CancellationToken ct) {
    var result = await Mediator.Send(command, ct);
    return ApiResponse.Success(result);
  }
}
