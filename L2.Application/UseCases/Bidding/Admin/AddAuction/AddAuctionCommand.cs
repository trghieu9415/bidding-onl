using L2.Application.Abstractions;

namespace L2.Application.UseCases.Bidding.Admin.AddAuction;

public record AddAuctionCommand(
  Guid CatalogItemId,
  decimal StepPrice,
  decimal ReservePrice
) : ICommand<Guid>;