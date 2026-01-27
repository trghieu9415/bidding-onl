using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidder.Admin.UnlockBidder;

public record UnlockBidderCommand(Guid Id) : ICommand<Unit>;
