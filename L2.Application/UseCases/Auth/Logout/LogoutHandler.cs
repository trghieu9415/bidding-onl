using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Logout;

public class LogoutHandler(
  IAuthService authService
) : IRequestHandler<LogoutCommand, Unit> {
  public async Task<Unit> Handle(LogoutCommand request, CancellationToken ct) {
    await authService.LogoutAsync(request.RefreshToken, request.AllDevices, ct);
    return Unit.Value;
  }
}
