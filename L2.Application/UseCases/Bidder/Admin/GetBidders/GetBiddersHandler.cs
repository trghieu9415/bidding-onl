using L2.Application.Models;
using L2.Application.Ports.Identity;
using MediatR;

namespace L2.Application.UseCases.Bidder.Admin.GetBidders;

public class GetBiddersHandler(IUserService userService) : IRequestHandler<GetBiddersQuery, GetBiddersResult> {
  public async Task<GetBiddersResult> Handle(GetBiddersQuery request, CancellationToken ct) {
    var (total, users) = await userService.GetAsync(request.SieveModel, UserRole.Bidder, ct);

    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetBiddersResult(users, meta);
  }
}