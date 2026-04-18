using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;
using Sieve.Models;

namespace L2.Application.UseCases.Bids.Queries.GetBiddingActivity;

public record GetBiddingActivityQuery(Guid UserId, SieveModel SieveModel) : IRequest<GetBiddingActivityResult>;

public record GetBiddingActivityResult(List<AuctionDto> Auctions, Meta Meta);
