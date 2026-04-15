using L2.Application.Abstractions;
using L2.Application.Models;
using Sieve.Models;

namespace L2.Application.UseCases.Bidder.GetBidders;

public record GetBiddersQuery(SieveModel SieveModel) : IQuery<GetBiddersResult>;
public record GetBiddersResult(List<User> Bidders, Meta Meta);
