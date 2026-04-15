using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Bidding.GetBidHistory;

public record GetBidHistoryQuery(Guid AuctionId, int Page, int PageSize) : IRequest<GetBidHistoryResult>;

public record GetBidHistoryResult(List<BidDto> Bids, Meta Meta);
