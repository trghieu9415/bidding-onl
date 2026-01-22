using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.UpdateAuction;

public record UpdateAuctionCommand(
  Guid Id,
  decimal StepPrice,
  decimal ReservePrice
) : ICommand<Unit>;