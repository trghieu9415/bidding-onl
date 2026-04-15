using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auctions.Commands.AddAuction;

public record AddAuctionCommand(
  Guid CatalogItemId,
  Guid SessionId,
  decimal StepPrice,
  decimal ReservePrice
) : IRequest<Guid>, ITransactional;
