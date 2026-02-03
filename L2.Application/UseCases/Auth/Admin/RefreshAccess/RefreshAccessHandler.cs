using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Admin.RefreshAccess;

public class RefreshAccessHandler(
  IAuthService authService
) : IRequestHandler<RefreshAccessCommand, RefreshAccessResult> {
  public async Task<RefreshAccessResult> Handle(RefreshAccessCommand request, CancellationToken ct) {
    var token = await authService.RefreshAsync(request.RefreshToken, ct);
    return new RefreshAccessResult(token);
  }
}
