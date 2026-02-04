using L0.API.Response;
using L2.Application.UseCases.Auth.Admin.ChangePassword;
using L2.Application.UseCases.Auth.Admin.Login;
using L2.Application.UseCases.Auth.Admin.Logout;
using L2.Application.UseCases.Auth.Admin.RefreshAccess;
using L2.Application.UseCases.Bidding.Bidder.Test;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers.Admin;

public class AuthController(IMediator mediator) : DashboardController {
  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginCommand command) {
    var result = await mediator.Send(command);
    return AppResponse.Success(result.Tokens, "Đăng nhập thành công");
  }

  [HttpPatch("change-password")]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command) {
    await mediator.Send(command);
    return AppResponse.Success("Đổi mật khẩu thành công");
  }

  [HttpPost("refresh")]
  public async Task<IActionResult> Refresh([FromBody] RefreshAccessCommand command) {
    var result = await mediator.Send(command);
    return AppResponse.Success(result.Tokens);
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout([FromBody] LogoutCommand command) {
    await mediator.Send(command);
    return AppResponse.Success("Đăng xuất thành công");
  }

  [HttpPost("test")]
  public async Task<IActionResult> Test([FromBody] TestCommand command) {
    await mediator.Send(command);
    return AppResponse.Success();
  }
}
