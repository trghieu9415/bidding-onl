using L2.Application.Filters;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Bidders.Queries.GetLockedBidders;

public record GetLockedBiddersQuery(UserFilter Filter) : IRequest<GetLockedBiddersResult>;

public record GetLockedBiddersResult(List<User> Bidders, Meta Meta);
