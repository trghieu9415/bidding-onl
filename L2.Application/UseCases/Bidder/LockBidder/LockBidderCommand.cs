using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidder.LockBidder;

public record LockBidderCommand(Guid Id) : IRequest<Unit>, ITransactional;
