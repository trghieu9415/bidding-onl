using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Bidders.Queries.GetBidder;

public record GetBidderQuery(Guid Id) : IRequest<GetBidderResult>;

public record GetBidderResult(User Bidder);
