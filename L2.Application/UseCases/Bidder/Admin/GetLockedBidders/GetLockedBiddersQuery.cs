using L2.Application.Abstractions;
using Sieve.Models;

namespace L2.Application.UseCases.Bidder.Admin.GetLockedBidders;

public record GetLockedBiddersQuery(SieveModel SieveModel) : IQuery<GetLockedBiddersResult>;