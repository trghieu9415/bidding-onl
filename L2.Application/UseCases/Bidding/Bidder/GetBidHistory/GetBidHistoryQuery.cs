using L2.Application.Abstractions;

namespace L2.Application.UseCases.Bidding.Bidder.GetBidHistory;

public record GetBidHistoryQuery(Guid AuctionId, int Page, int PageSize) : IQuery<GetBidHistoryResult>;
