using L2.Application.DTOs;
using L2.Application.Models;

namespace L2.Application.UseCases.Bidding.Bidder.GetBidHistory;

public record GetBidHistoryResult(List<BidDto> Bids, Meta Meta);
