using L2.Application.Models;
using MediatR;
using Sieve.Models;

namespace L2.Application.UseCases.Bidder.GetLockedBidders;

public record GetLockedBiddersQuery(SieveModel SieveModel) : IRequest<GetLockedBiddersResult>;

public record GetLockedBiddersResult(List<User> Bidders, Meta Meta);
