using L2.Application.Models;
using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Bidders.Queries.GetLockedBidders;

public class GetLockedBiddersHandler(IUserService userService)
  : IRequestHandler<GetLockedBiddersQuery, GetLockedBiddersResult> {
  public async Task<GetLockedBiddersResult> Handle(GetLockedBiddersQuery request, CancellationToken ct) {
    var (total, users) = await userService.GetAsync(request.Filter, UserRole.Bidder, ct);
    var lockedUsers = users.Where(u => !u.IsActive).ToList();

    var meta = Meta.Create(request.Filter.Page, request.Filter.PerPage, total);
    return new GetLockedBiddersResult(lockedUsers, meta);
  }
}
