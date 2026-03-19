using L0.API.Response;
using L2.Application.UseCases.Auth.Admin.ChangePassword;
using L2.Application.UseCases.Auth.Admin.Login;
using L2.Application.UseCases.Auth.Admin.Logout;
using L2.Application.UseCases.Auth.Admin.RefreshAccess;
using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers.Admin;

public class AuthController : DashboardController {
  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginCommand command) {
    var result = await Mediator.Send(command);
    return AppResponse.Success(result.Tokens, "Đăng nhập thành công");
  }

  [HttpPatch("change-password")]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command) {
    await Mediator.Send(command);
    return AppResponse.Success("Đổi mật khẩu thành công");
  }

  [HttpPost("refresh")]
  public async Task<IActionResult> Refresh([FromBody] RefreshAccessCommand command) {
    var result = await Mediator.Send(command);
    return AppResponse.Success(result.Tokens);
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout([FromBody] LogoutCommand command) {
    await Mediator.Send(command);
    return AppResponse.Success("Đăng xuất thành công");
  }
}
