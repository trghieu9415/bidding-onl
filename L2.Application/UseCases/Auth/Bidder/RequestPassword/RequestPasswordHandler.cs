using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Bidder.RequestPassword;

public class RequestPasswordHandler(IAuthService authService) : IRequestHandler<RequestPasswordCommand, Unit> {
  public async Task<Unit> Handle(RequestPasswordCommand request, CancellationToken ct) {
    await authService.RequestPasswordAsync(request.Email, ct);
    return Unit.Value;
  }
}