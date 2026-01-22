using L2.Application.Exceptions;
using L2.Application.Models;
using L2.Application.Ports.Identity;
using MediatR;

namespace L2.Application.UseCases.Bidder.Admin.GetBidder;

public class GetBidderHandler(IUserService userService) : IRequestHandler<GetBidderQuery, GetBidderResult> {
  public async Task<GetBidderResult> Handle(GetBidderQuery request, CancellationToken ct) {
    var user = await userService.GetByIdAsync(request.Id, UserRole.Bidder, ct)
               ?? throw new AppException("Không tìm thấy người dùng", 404);

    return new GetBidderResult(user);
  }
}