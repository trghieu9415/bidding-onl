using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Bids.Queries.GetBidHistory;

public record GetBidHistoryQuery(Guid AuctionId, int Page, int PerPage) : IRequest<GetBidHistoryResult>;

public record GetBidHistoryResult(List<BidDto> Bids, Meta Meta);
