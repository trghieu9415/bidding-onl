using L2.Application.Exceptions;
using L2.Application.Models;
using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Bidders.Queries.GetBidder;

public class GetBidderHandler(IUserService userService) : IRequestHandler<GetBidderQuery, GetBidderResult> {
  public async Task<GetBidderResult> Handle(GetBidderQuery request, CancellationToken ct) {
    var user = await userService.GetByIdAsync(request.Id, UserRole.Bidder, ct)
               ?? throw new WorkflowException("Không tìm thấy người dùng", 404);

    return new GetBidderResult(user);
  }
}
