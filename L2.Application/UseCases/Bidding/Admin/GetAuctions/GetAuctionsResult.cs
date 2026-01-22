using L2.Application.DTOs;
using L2.Application.Models;

namespace L2.Application.UseCases.Bidding.Admin.GetAuctions;

public record GetAuctionsResult(List<AuctionDto> Auctions, Meta Meta);