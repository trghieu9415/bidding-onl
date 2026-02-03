using L0.API.Response;
using L2.Application.UseCases.Auth.Bidder.ChangePassword;
using L2.Application.UseCases.Auth.Bidder.GetProfile;
using L2.Application.UseCases.Auth.Bidder.Login;
using L2.Application.UseCases.Auth.Bidder.Logout;
using L2.Application.UseCases.Auth.Bidder.RefreshAccess;
using L2.Application.UseCases.Auth.Bidder.Register;
using L2.Application.UseCases.Auth.Bidder.RequestPassword;
using L2.Application.UseCases.Auth.Bidder.ResetPassword;
using L2.Application.UseCases.Auth.Bidder.UpdateProfile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers.Bidder;

public class AuthController(IMediator mediator) : UserController {
  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterCommand command) {
    var result = await mediator.Send(command);
    return AppResponse.Success(result.Tokens, "Đăng ký tài khoản thành công");
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginCommand command) {
    var result = await mediator.Send(command);
    return AppResponse.Success(result.Tokens, "Đăng nhập thành công");
  }

  [HttpGet("profile")]
  public async Task<IActionResult> GetProfile() {
    var result = await mediator.Send(new GetProfileQuery());
    return AppResponse.Success(result.Profile);
  }

  [HttpPut("profile")]
  public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command) {
    await mediator.Send(command);
    return AppResponse.Success("Cập nhật thông tin cá nhân thành công");
  }

  [HttpPost("change-password")]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command) {
    await mediator.Send(command);
    return AppResponse.Success("Đổi mật khẩu thành công");
  }

  [HttpPost("forgot-password")]
  public async Task<IActionResult> ForgotPassword([FromBody] RequestPasswordCommand command) {
    await mediator.Send(command);
    return AppResponse.Success("Yêu cầu khôi phục mật khẩu đã được gửi đến Email");
  }

  [HttpPost("reset-password")]
  public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command) {
    await mediator.Send(command);
    return AppResponse.Success("Mật khẩu đã được đặt lại thành công");
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
}
