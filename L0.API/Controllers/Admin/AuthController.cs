using L0.API.Response;
using L2.Application.UseCases.Auth.Admin.ChangePassword;
using L2.Application.UseCases.Auth.Admin.Login;
using L2.Application.UseCases.Auth.Admin.Logout;
using L2.Application.UseCases.Auth.Admin.RefreshAccess;
using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers.Admin;

public class AuthController : DashboardController {
  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct) {
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

  [HttpPatch("change-password")]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return AppResponse.Success("Đổi mật khẩu thành công");
  }

  [HttpPost("refresh")]
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
    return AppResponse.Success(tokens.Access, "Làm mới thành công");
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return AppResponse.Success("Đăng xuất thành công");
  }
}
