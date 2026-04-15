using L2.Application.Abstractions;
using L2.Application.Models;

namespace L2.Application.UseCases.Bidder.GetBidder;

public record GetBidderQuery(Guid Id) : IQuery<GetBidderResult>;
public record GetBidderResult(User Bidder);
