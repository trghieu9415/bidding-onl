using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.RequestPassword;

public class RequestPasswordHandler(IAuthService authService) : IRequestHandler<RequestPasswordCommand, bool> {
  public async Task<bool> Handle(RequestPasswordCommand request, CancellationToken ct) {
    await authService.RequestPasswordAsync(request.Email, ct);
    return true;
  }
}
