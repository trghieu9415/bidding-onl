using L2.Application.Abstractions;
using Sieve.Models;

namespace L2.Application.UseCases.Bidding.Admin.GetRemovedSessions;

public record GetRemovedSessionsQuery(SieveModel SieveModel) : IQuery<GetRemovedSessionsResult>;