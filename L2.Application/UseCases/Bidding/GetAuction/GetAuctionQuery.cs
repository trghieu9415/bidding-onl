using L2.Application.Abstractions;
using L2.Application.DTOs;

namespace L2.Application.UseCases.Bidding.GetAuction;

public record GetAuctionQuery(Guid Id) : IQuery<GetAuctionResult>;
public record GetAuctionResult(AuctionDto Auction);
