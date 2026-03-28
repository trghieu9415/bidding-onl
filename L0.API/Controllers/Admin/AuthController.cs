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
    return AppResponse.Success(result.Tokens, "Đăng nhập thành công");
  }

  [HttpPatch("change-password")]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return AppResponse.Success("Đổi mật khẩu thành công");
  }

  [HttpPost("refresh")]
  public async Task<IActionResult> Refresh([FromBody] RefreshAccessCommand command, CancellationToken ct) {
    var result = await Mediator.Send(command, ct);
    return AppResponse.Success(result.Tokens);
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken ct) {
    await Mediator.Send(command, ct);
    return AppResponse.Success("Đăng xuất thành công");
  }
}
