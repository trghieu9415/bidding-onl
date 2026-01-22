using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Bidder.ChangePassword;

public class ChangePasswordHandler(
  IAuthentication authService,
  ICurrentUser currentUser
) : IRequestHandler<ChangePasswordCommand, Unit> {
  public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken ct) {
    var userId = currentUser.User.Id;

    await authService.ChangePasswordAsync(
      userId,
      request.OldPassword,
      request.NewPassword,
      ct
    );

    return Unit.Value;
  }
}