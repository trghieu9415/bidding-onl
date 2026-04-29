using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Bidders.Commands.LockBidder;

public class LockBidderHandler(IUserService userService) : IRequestHandler<LockBidderCommand, bool> {
  public async Task<bool> Handle(LockBidderCommand request, CancellationToken ct) {
    await userService.LockAsync(request.Id, ct);
    return true;
  }
}
