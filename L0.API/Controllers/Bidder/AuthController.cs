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
  [AllowAnonymous]
  public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct) {
    var result = await Mediator.Send(command, ct);
    return AppResponse.Success(result.Tokens, "Đăng ký tài khoản thành công");
  }

  [HttpPost("login")]
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
    return AppResponse.Success(tokens.Access, "Đăng nhập thành công");
  }

  [HttpGet("profile")]
  public async Task<IActionResult> GetProfile(CancellationToken ct) {
    var result = await Mediator.Send(new GetProfileQuery(), ct);
    return AppResponse.Success(result.Profile);
  }

  [HttpPut("profile")]
  public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return AppResponse.Success("Cập nhật thông tin cá nhân thành công");
  }

  [HttpPost("change-password")]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return AppResponse.Success("Đổi mật khẩu thành công");
  }

  [HttpPost("forgot-password")]
  public async Task<IActionResult> ForgotPassword([FromBody] RequestPasswordCommand command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return AppResponse.Success("Yêu cầu khôi phục mật khẩu đã được gửi đến Email");
  }

  [HttpPost("reset-password")]
  public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return AppResponse.Success("Mật khẩu đã được đặt lại thành công");
  }

  [HttpPost("refresh")]
  public async Task<IActionResult> Refresh([FromBody] RefreshAccessCommand command, CancellationToken ct) {
    var result = await Mediator.Send(command, ct);
    var tokens = result.Tokens;

    var cookieOptions = new CookieOptions {
      Expires = tokens.Refresh.ExpiredAt,
      Secure = true,
      SameSite = SameSiteMode.None,
      HttpOnly = true
    };

    Response.Cookies.Append("Refresh", tokens.Refresh.Token, cookieOptions);
    return AppResponse.Success(tokens.Access, "Làm mới thành công");
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return AppResponse.Success("Đăng xuất thành công");
  }

  [HttpPost("test")]
  [AllowAnonymous]
  public async Task<IActionResult> Test([FromBody] TestCommand command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return AppResponse.Success();
  }
}
