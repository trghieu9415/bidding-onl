using L2.Application.Abstractions;
using L2.Application.DTOs;
using L2.Application.Models;

namespace L2.Application.UseCases.Bidding.GetBidHistory;

public record GetBidHistoryQuery(Guid AuctionId, int Page, int PageSize) : IQuery<GetBidHistoryResult>;
public record GetBidHistoryResult(List<BidDto> Bids, Meta Meta);
