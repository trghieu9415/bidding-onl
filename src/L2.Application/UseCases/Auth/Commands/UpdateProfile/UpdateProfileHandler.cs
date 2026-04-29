using L2.Application.Exceptions;
using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.UpdateProfile;

public class UpdateProfileHandler(IUserService userService)
  : IRequestHandler<UpdateProfileCommand, bool> {
  public async Task<bool> Handle(UpdateProfileCommand request, CancellationToken ct) {
    var user =
      await userService.GetByIdAsync(request.UserId, request.Role, ct)
      ?? throw new WorkflowException($"Không tìm thấy người dùng - Id:{request.UserId}");

    var data = request.Data;
    user = user with {
      FullName = data.FullName,
      PhoneNumber = data.PhoneNumber,
      Url = data.Url
    };

    await userService.UpdateAsync(user, ct);
    return true;
  }
}
