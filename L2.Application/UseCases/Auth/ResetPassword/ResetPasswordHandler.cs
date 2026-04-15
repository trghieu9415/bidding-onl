using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.ResetPassword;

public class ResetPasswordHandler(IAuthService authService) : IRequestHandler<ResetPasswordCommand, Unit> {
  public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken ct) {
    await authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword, ct);
    return Unit.Value;
  }
}
