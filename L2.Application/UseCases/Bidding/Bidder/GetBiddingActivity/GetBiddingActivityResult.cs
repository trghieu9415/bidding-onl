using L2.Application.DTOs;
using L2.Application.Models;

namespace L2.Application.UseCases.Bidding.Bidder.GetBiddingActivity;

public record GetBiddingActivityResult(List<AuctionDto> Auctions, Meta Meta);
