using L2.Application.Ports.Identity;
using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Bidder.UpdateProfile;

public class UpdateProfileHandler(IUserService userService, ICurrentUser currentUser)
  : IRequestHandler<UpdateProfileCommand, Unit> {
  public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken ct) {
    var userUpdate = currentUser.User with {
      FullName = request.FullName,
      PhoneNumber = request.PhoneNumber,
      Url = request.Url
    };

    await userService.UpdateAsync(userUpdate, ct);
    return Unit.Value;
  }
}