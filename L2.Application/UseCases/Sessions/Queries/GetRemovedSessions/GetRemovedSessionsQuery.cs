using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Sessions.Queries.GetRemovedSessions;

public record GetRemovedSessionsQuery(SessionFilter Filter) : IRequest<GetRemovedSessionsResult>;

public record GetRemovedSessionsResult(List<AuctionSessionDto> Sessions, Meta Meta);
