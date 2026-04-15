using L2.Application.Abstractions;
using L2.Application.DTOs;
using L2.Application.Models;
using Sieve.Models;

namespace L2.Application.UseCases.Bidding.GetRemovedSessions;

public record GetRemovedSessionsQuery(SieveModel SieveModel) : IQuery<GetRemovedSessionsResult>;
public record GetRemovedSessionsResult(List<AuctionSessionDto> Sessions, Meta Meta);
