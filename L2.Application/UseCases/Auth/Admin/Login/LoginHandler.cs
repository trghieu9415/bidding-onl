using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Admin.Login;

public class LoginHandler(IAuthService authService) : IRequestHandler<LoginCommand, LoginResult> {
  public async Task<LoginResult> Handle(LoginCommand request, CancellationToken ct) {
    var tokens = await authService.LoginAdminAsync(request.Email, request.Password, ct);
    return new LoginResult(tokens);
  }
}