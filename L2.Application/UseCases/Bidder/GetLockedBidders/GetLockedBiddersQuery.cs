using L2.Application.Abstractions;
using L2.Application.Models;
using Sieve.Models;

namespace L2.Application.UseCases.Bidder.GetLockedBidders;

public record GetLockedBiddersQuery(SieveModel SieveModel) : IQuery<GetLockedBiddersResult>;
public record GetLockedBiddersResult(List<User> Bidders, Meta Meta);
