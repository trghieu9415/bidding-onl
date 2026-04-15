using L2.Application.Abstractions;
using L2.Application.DTOs;

namespace L2.Application.UseCases.Bidding.GetSession;

public record GetSessionQuery(Guid Id) : IQuery<GetSessionResult>;
public record GetSessionResult(AuctionSessionDto Session);
