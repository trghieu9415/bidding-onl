using L2.Application.DTOs;
using MediatR;

namespace L2.Application.UseCases.Sessions.Queries.GetCurrentSession;

public record GetCurrentSessionQuery : IRequest<GetCurrentSessionResult>;

public record GetCurrentSessionResult(List<AuctionSessionDto> Sessions);
