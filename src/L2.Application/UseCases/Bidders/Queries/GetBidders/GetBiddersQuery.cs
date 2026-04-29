using L2.Application.Filters;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Bidders.Queries.GetBidders;

public record GetBiddersQuery(UserFilter Filter) : IRequest<GetBiddersResult>;

public record GetBiddersResult(List<User> Bidders, Meta Meta);
