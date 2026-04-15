using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;
using Sieve.Models;

namespace L2.Application.UseCases.Bidding.GetAuctions;

public record GetAuctionsQuery(SieveModel SieveModel) : IRequest<GetAuctionsResult>;

public record GetAuctionsResult(List<AuctionDto> Auctions, Meta Meta);
