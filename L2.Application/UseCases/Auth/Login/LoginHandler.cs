using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Login;

public class LoginHandler(
  IAuthService authService
) : IRequestHandler<LoginCommand, LoginResult> {
  public async Task<LoginResult> Handle(
    LoginCommand request,
    CancellationToken ct
  ) {
    var tokens = await authService.LoginAsync(request.Email, request.Password, request.Role, ct);
    return new LoginResult(tokens);
  }
}
