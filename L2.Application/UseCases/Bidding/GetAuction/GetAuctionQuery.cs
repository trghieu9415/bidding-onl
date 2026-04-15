using L2.Application.DTOs;
using MediatR;

namespace L2.Application.UseCases.Bidding.GetAuction;

public record GetAuctionQuery(Guid Id) : IRequest<GetAuctionResult>;

public record GetAuctionResult(AuctionDto Auction);
