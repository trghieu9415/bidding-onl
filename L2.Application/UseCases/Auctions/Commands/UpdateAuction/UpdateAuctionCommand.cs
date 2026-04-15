using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auctions.Commands.UpdateAuction;

public record UpdateAuctionCommand(
  Guid Id,
  UpdateAuctionRequest Data
) : IRequest<Unit>, ITransactional;

public record UpdateAuctionRequest(
  decimal StepPrice,
  decimal ReservePrice
);
