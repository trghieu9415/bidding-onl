using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;
using Sieve.Models;

namespace L2.Application.UseCases.Bidding.GetWonAuctions;

public record GetWonAuctionsQuery(SieveModel SieveModel) : IRequest<GetWonAuctionsResult>;

public record GetWonAuctionsResult(List<AuctionDto> Auctions, Meta Meta);
