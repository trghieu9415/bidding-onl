using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Admin.ChangePassword;

public class ChangePasswordHandler(IAuthentication authService, ICurrentUser currentUser)
  : IRequestHandler<ChangePasswordCommand, Unit> {
  public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken ct) {
    await authService.ChangePasswordAsync(currentUser.User.Id, request.OldPassword, request.NewPassword, ct);
    return Unit.Value;
  }
}