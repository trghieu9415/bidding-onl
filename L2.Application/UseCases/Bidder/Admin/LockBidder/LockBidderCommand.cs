using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidder.Admin.LockBidder;

public record LockBidderCommand(Guid Id) : ICommand<Unit>;