using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;
using Sieve.Models;

namespace L2.Application.UseCases.Bidding.GetRemovedAuctions;

public record GetRemovedAuctionsQuery(SieveModel SieveModel) : IRequest<GetRemovedAuctionsResult>;

public record GetRemovedAuctionsResult(List<AuctionDto> Auctions, Meta Meta);
