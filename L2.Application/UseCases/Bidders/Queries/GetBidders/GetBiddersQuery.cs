using L2.Application.Models;
using MediatR;
using Sieve.Models;

namespace L2.Application.UseCases.Bidders.Queries.GetBidders;

public record GetBiddersQuery(SieveModel SieveModel) : IRequest<GetBiddersResult>;

public record GetBiddersResult(List<User> Bidders, Meta Meta);
