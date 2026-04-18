using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.ChangePassword;

public class ChangePasswordHandler(
  IAuthService authService
) : IRequestHandler<ChangePasswordCommand, bool> {
  public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken ct) {
    var userId = request.UserId;
    var data = request.Data;

    await authService.ChangePasswordAsync(userId, data.OldPassword, data.NewPassword, ct);
    return true;
  }
}
