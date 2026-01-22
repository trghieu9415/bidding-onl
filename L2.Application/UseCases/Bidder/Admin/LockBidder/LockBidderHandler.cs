using L2.Application.Ports.Identity;
using MediatR;

namespace L2.Application.UseCases.Bidder.Admin.LockBidder;

public class LockBidderHandler(IUserService userService) : IRequestHandler<LockBidderCommand, Unit> {
  public async Task<Unit> Handle(LockBidderCommand request, CancellationToken ct) {
    await userService.LockAsync(request.Id, ct);
    return Unit.Value;
  }
}