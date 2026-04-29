using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.ResetPassword;

public class ResetPasswordHandler(IAuthService authService) : IRequestHandler<ResetPasswordCommand, bool> {
  public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken ct) {
    await authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword, ct);
    return true;
  }
}
