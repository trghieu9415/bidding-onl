using L0.API.Response;
using L2.Application.Models;
using L2.Application.UseCases.Auth.Commands.ChangePassword;
using L2.Application.UseCases.Auth.Commands.Login;
using L2.Application.UseCases.Auth.Commands.Logout;
using L2.Application.UseCases.Auth.Commands.RefreshAccess;
using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers.Admin;

public class AuthController : DashboardController {
  [HttpPost("login")]
  [ProducesSuccess<TokenModel>]
  public async Task<IActionResult> Login([FromBody] LoginRequest data, CancellationToken ct) {
    var command = new LoginCommand(data, UserRole.Admin);
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

  [HttpPatch("change-password")]
  [ProducesSuccess<bool>]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken ct) {
    var result = await Mediator.Send(command, ct);
    return ApiResponse.Success(result, "Đổi mật khẩu thành công");
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
    var result = await Mediator.Send(command, ct);
    return ApiResponse.Success(result, "Đăng xuất thành công");
  }
}
