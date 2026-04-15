using L2.Application.Abstractions;
using L2.Application.DTOs;
using L2.Application.Models;
using Sieve.Models;

namespace L2.Application.UseCases.Bidding.GetRemovedAuctions;

public record GetRemovedAuctionsQuery(SieveModel SieveModel) : IQuery<GetRemovedAuctionsResult>;
public record GetRemovedAuctionsResult(List<AuctionDto> Auctions, Meta Meta);
