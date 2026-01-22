using L2.Application.Abstractions;

namespace L2.Application.UseCases.Bidding.Bidder.PlaceBid;

public record PlaceBidCommand(Guid AuctionId, decimal Amount) : ICommand<Guid>;