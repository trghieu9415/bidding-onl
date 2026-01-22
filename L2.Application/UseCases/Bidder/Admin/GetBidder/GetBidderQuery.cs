using L2.Application.Abstractions;

namespace L2.Application.UseCases.Bidder.Admin.GetBidder;

public record GetBidderQuery(Guid Id) : IQuery<GetBidderResult>;