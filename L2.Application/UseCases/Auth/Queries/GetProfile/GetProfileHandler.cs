using L2.Application.Exceptions;
using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Queries.GetProfile;

public class GetProfileHandler(IUserService userService)
  : IRequestHandler<GetProfileQuery, GetProfileResult> {
  public async Task<GetProfileResult> Handle(GetProfileQuery request, CancellationToken ct) {
    var user =
      await userService.GetByIdAsync(request.Id, request.Role, ct)
      ?? throw new WorkflowException("Người dùng không tồn tại", 404);

    return new GetProfileResult(user);
  }
}
