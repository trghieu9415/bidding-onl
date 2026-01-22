using L2.Application.Abstractions;
using Sieve.Models;

namespace L2.Application.UseCases.Bidding.Admin.GetSessions;

public record GetSessionsQuery(SieveModel SieveModel) : IQuery<GetSessionsResult>;