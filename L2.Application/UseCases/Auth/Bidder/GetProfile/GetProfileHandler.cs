using L2.Application.Exceptions;
using L2.Application.Models;
using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Bidder.GetProfile;

public class GetProfileHandler(IUserService userService, ICurrentUser currentUser)
  : IRequestHandler<GetProfileQuery, GetProfileResult> {
  public async Task<GetProfileResult> Handle(GetProfileQuery request, CancellationToken ct) {
    var user = await userService.GetByIdAsync(currentUser.Id, UserRole.Bidder, ct)
               ?? throw new WorkflowException("Người dùng không tồn tại", 404);

    return new GetProfileResult(user);
  }
}
