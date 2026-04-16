using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidders.Commands.LockBidder;

public record LockBidderCommand(Guid Id) : IRequest<bool>, ITransactional;
