using L2.Application.Abstractions;
using Sieve.Models;

namespace L2.Application.UseCases.Bidding.Admin.GetAuctions;

public record GetAuctionsQuery(SieveModel SieveModel) : IQuery<GetAuctionsResult>;