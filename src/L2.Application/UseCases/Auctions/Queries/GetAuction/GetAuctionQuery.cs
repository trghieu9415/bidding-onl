using L2.Application.DTOs;
using MediatR;

namespace L2.Application.UseCases.Auctions.Queries.GetAuction;

public record GetAuctionQuery(Guid Id) : IRequest<GetAuctionResult>;

public record GetAuctionResult(AuctionDto Auction);
