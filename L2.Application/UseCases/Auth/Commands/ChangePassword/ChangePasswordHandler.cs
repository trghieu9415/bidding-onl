using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.ChangePassword;

public class ChangePasswordHandler(
  IAuthService authService,
  ICurrentUser currentUser
) : IRequestHandler<ChangePasswordCommand, bool> {
  public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken ct) {
    var userId = currentUser.Id;

    await authService.ChangePasswordAsync(
      userId,
      request.OldPassword,
      request.NewPassword,
      ct
    );

    return true;
  }
}
