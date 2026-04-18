using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Sessions.Queries.GetSessions;

public record GetSessionsQuery(SessionFilter Filter) : IRequest<GetSessionsResult>;

public record GetSessionsResult(List<AuctionSessionDto> Sessions, Meta Meta);
