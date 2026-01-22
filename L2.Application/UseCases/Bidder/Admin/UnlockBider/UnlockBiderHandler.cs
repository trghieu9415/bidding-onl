using L2.Application.Ports.Identity;
using MediatR;

namespace L2.Application.UseCases.Bidder.Admin.UnlockBider;

public class UnlockBidderHandler(IUserService userService) : IRequestHandler<UnlockBidderCommand, Unit> {
  public async Task<Unit> Handle(UnlockBidderCommand request, CancellationToken ct) {
    await userService.UnlockAsync(request.Id, ct);
    return Unit.Value;
  }
}