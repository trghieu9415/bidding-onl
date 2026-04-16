using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Bidders.Commands.UnlockBidder;

public class UnlockBidderHandler(IUserService userService) : IRequestHandler<UnlockBidderCommand, bool> {
  public async Task<bool> Handle(UnlockBidderCommand request, CancellationToken ct) {
    await userService.UnlockAsync(request.Id, ct);
    return true;
  }
}
