using L2.Application.DTOs;
using L2.Application.Models;

namespace L2.Application.UseCases.Bidding.Admin.GetRemovedAuctions;

public record GetRemovedAuctionsResult(List<AuctionDto> Auctions, Meta Meta);