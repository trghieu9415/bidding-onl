using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.Logout;

public class LogoutHandler(
  IAuthService authService
) : IRequestHandler<LogoutCommand, bool> {
  public async Task<bool> Handle(LogoutCommand request, CancellationToken ct) {
    await authService.LogoutAsync(request.RefreshToken, request.AllDevices, ct);
    return true;
  }
}
