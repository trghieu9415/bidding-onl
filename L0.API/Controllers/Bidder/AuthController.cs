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

namespace L0.API.Controllers.Bidder;

public class AuthController : UserController {
  [HttpPost("register")]
  [ProducesSuccess<TokenModel>]
  [AllowAnonymous]
  public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct) {
    var result = await Mediator.Send(command, ct);
    return ApiResponse.Success(result.Tokens, "Đăng ký tài khoản thành công");
  }

  [HttpPost("login")]
  [ProducesSuccess<TokenModel>]
  [AllowAnonymous]
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
    var result = await Mediator.Send(new GetProfileQuery(), ct);
    return ApiResponse.Success(result.Profile);
  }

  [HttpPut("profile")]
  [ProducesSuccess<bool>]
  public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return ApiResponse.Success("Cập nhật thông tin cá nhân thành công");
  }

  [HttpPost("change-password")]
  [ProducesSuccess<bool>]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return ApiResponse.Success("Đổi mật khẩu thành công");
  }

  [HttpPost("forgot-password")]
  [ProducesSuccess<bool>]
  [AllowAnonymous]
  public async Task<IActionResult> ForgotPassword([FromBody] RequestPasswordCommand command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return ApiResponse.Success("Yêu cầu khôi phục mật khẩu đã được gửi đến Email");
  }

  [HttpPost("reset-password")]
  [ProducesSuccess<bool>]
  public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return ApiResponse.Success("Mật khẩu đã được đặt lại thành công");
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
  public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return ApiResponse.Success("Đăng xuất thành công");
  }

  [HttpPost("test")]
  [AllowAnonymous]
  public async Task<IActionResult> Test([FromBody] TestCommand command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return ApiResponse.Success();
  }
}
