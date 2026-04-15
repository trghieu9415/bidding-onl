using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidders.Commands.UnlockBidder;

public record UnlockBidderCommand(Guid Id) : IRequest<Unit>, ITransactional;
