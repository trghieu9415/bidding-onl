using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Bids.Queries.GetBiddingActivity;

public record GetBiddingActivityQuery(Guid UserId, AuctionFilter Filter) : IRequest<GetBiddingActivityResult>;

public record GetBiddingActivityResult(List<AuctionDto> Auctions, Meta Meta);
