using L2.Application.Abstractions;
using Sieve.Models;

namespace L2.Application.UseCases.Bidder.Admin.GetBidders;

public record GetBiddersQuery(SieveModel SieveModel) : IQuery<GetBiddersResult>;