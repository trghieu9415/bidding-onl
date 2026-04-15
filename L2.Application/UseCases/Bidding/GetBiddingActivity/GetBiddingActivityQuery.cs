using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;
using Sieve.Models;

namespace L2.Application.UseCases.Bidding.GetBiddingActivity;

public record GetBiddingActivityQuery(SieveModel SieveModel) : IRequest<GetBiddingActivityResult>;

public record GetBiddingActivityResult(List<AuctionDto> Auctions, Meta Meta);
