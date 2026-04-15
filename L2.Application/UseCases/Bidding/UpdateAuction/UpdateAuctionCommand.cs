using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.UpdateAuction;

public record UpdateAuctionCommand(
  Guid Id,
  decimal StepPrice,
  decimal ReservePrice
) : IRequest<Unit>, ITransactional;
