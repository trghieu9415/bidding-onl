using L2.Application.Abstractions;
using Sieve.Models;

namespace L2.Application.UseCases.Bidding.Bidder.GetSessions;

public record GetSessionsQuery(SieveModel SieveModel) : IQuery<GetSessionsResult>;