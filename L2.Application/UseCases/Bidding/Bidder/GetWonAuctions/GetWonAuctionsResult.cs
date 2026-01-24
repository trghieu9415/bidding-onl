using L2.Application.DTOs;
using L2.Application.Models;

namespace L2.Application.UseCases.Bidding.Bidder.GetWonAuctions;

public record GetWonAuctionsResult(List<AuctionDto> Auctions, Meta Meta);
