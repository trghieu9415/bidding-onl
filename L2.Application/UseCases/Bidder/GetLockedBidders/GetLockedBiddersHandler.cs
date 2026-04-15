using L2.Application.Models;
using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Bidder.GetLockedBidders;

public class GetLockedBiddersHandler(IUserService userService)
  : IRequestHandler<GetLockedBiddersQuery, GetLockedBiddersResult> {
  public async Task<GetLockedBiddersResult> Handle(GetLockedBiddersQuery request, CancellationToken ct) {
    var (total, users) = await userService.GetAsync(request.SieveModel, UserRole.Bidder, ct);
    var lockedUsers = users.Where(u => !u.IsActive).ToList();

    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetLockedBiddersResult(lockedUsers, meta);
  }
}
