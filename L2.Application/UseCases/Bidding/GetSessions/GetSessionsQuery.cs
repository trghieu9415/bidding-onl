using L2.Application.Abstractions;
using L2.Application.DTOs;
using L2.Application.Models;
using Sieve.Models;

namespace L2.Application.UseCases.Bidding.GetSessions;

public record GetSessionsQuery(SieveModel SieveModel) : IQuery<GetSessionsResult>;
public record GetSessionsResult(List<AuctionSessionDto> Sessions, Meta Meta);
