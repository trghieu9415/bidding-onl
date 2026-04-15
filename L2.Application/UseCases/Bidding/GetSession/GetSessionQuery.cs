using L2.Application.DTOs;
using MediatR;

namespace L2.Application.UseCases.Bidding.GetSession;

public record GetSessionQuery(Guid Id) : IRequest<GetSessionResult>;

public record GetSessionResult(AuctionSessionDto Session);
