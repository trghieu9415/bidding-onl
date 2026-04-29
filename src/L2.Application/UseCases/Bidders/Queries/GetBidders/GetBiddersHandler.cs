using L2.Application.Models;
using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Bidders.Queries.GetBidders;

public class GetBiddersHandler(IUserService userService) : IRequestHandler<GetBiddersQuery, GetBiddersResult> {
  public async Task<GetBiddersResult> Handle(GetBiddersQuery request, CancellationToken ct) {
    var (total, users) = await userService.GetAsync(request.Filter, UserRole.Bidder, ct);

    var meta = Meta.Create(request.Filter.Page, request.Filter.PerPage, total);
    return new GetBiddersResult(users, meta);
  }
}
