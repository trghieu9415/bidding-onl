using L2.Application.Abstractions;
using Sieve.Models;

namespace L2.Application.UseCases.Bidding.Admin.GetRemovedAuctions;

public record GetRemovedAuctionsQuery(SieveModel SieveModel) : IQuery<GetRemovedAuctionsResult>;