using L2.Application.Abstractions;

namespace L2.Application.UseCases.Bidding.Bidder.GetAuction;

public record GetAuctionQuery(Guid Id) : IQuery<GetAuctionResult>;