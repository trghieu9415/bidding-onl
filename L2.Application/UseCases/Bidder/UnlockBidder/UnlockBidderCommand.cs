using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidder.UnlockBidder;

public record UnlockBidderCommand(Guid Id) : IRequest<Unit>, ITransactional;
