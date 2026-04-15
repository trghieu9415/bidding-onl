using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;
using Sieve.Models;

namespace L2.Application.UseCases.Sessions.Queries.GetRemovedSessions;

public record GetRemovedSessionsQuery(SieveModel SieveModel) : IRequest<GetRemovedSessionsResult>;

public record GetRemovedSessionsResult(List<AuctionSessionDto> Sessions, Meta Meta);
