using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.AddAuction;

public record AddAuctionCommand(
  Guid CatalogItemId,
  decimal StepPrice,
  decimal ReservePrice
) : IRequest<Guid>, ITransactional;
