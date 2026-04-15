using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Bidder.UnlockBidder;

public class UnlockBidderHandler(IUserService userService) : IRequestHandler<UnlockBidderCommand, Unit> {
  public async Task<Unit> Handle(UnlockBidderCommand request, CancellationToken ct) {
    await userService.UnlockAsync(request.Id, ct);
    return Unit.Value;
  }
}
