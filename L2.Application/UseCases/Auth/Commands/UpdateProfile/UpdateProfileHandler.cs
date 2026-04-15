using L2.Application.Exceptions;
using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.UpdateProfile;

public class UpdateProfileHandler(IUserService userService, ICurrentUser currentUser)
  : IRequestHandler<UpdateProfileCommand, Unit> {
  public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken ct) {
    var user =
      await userService.GetByIdAsync(currentUser.Id, currentUser.Role, ct)
      ?? throw new WorkflowException($"Không tìm thấy người dùng - Id:{currentUser.Id}");

    user = user with {
      FullName = request.FullName,
      PhoneNumber = request.PhoneNumber,
      Url = request.Url
    };

    await userService.UpdateAsync(user, ct);
    return Unit.Value;
  }
}
