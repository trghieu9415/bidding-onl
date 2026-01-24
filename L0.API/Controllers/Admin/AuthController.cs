using L0.API.Response;
using L2.Application.UseCases.Auth.Admin.ChangePassword;
using L2.Application.UseCases.Auth.Admin.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers.Admin;

public class AuthController(IMediator mediator) : DashboardController {
  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginCommand command) {
    var result = await mediator.Send(command);
    return AppResponse.Success(result, "Đăng nhập thành công");
  }

  [HttpPost("change-password")]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command) {
    await mediator.Send(command);
    return AppResponse.Success("Đổi mật khẩu thành công");
  }
}
