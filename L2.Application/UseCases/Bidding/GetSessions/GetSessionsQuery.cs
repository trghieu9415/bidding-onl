using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;
using Sieve.Models;

namespace L2.Application.UseCases.Bidding.GetSessions;

public record GetSessionsQuery(SieveModel SieveModel) : IRequest<GetSessionsResult>;

public record GetSessionsResult(List<AuctionSessionDto> Sessions, Meta Meta);
