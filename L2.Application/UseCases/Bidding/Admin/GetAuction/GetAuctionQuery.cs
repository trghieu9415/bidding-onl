using L2.Application.Abstractions;

namespace L2.Application.UseCases.Bidding.Admin.GetAuction;

public record GetAuctionQuery(Guid Id) : IQuery<GetAuctionResult>;