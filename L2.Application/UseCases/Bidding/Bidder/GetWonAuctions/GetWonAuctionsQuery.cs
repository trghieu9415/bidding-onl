using L2.Application.Abstractions;
using Sieve.Models;

namespace L2.Application.UseCases.Bidding.Bidder.GetWonAuctions;

public record GetWonAuctionsQuery(SieveModel SieveModel) : IQuery<GetWonAuctionsResult>;
