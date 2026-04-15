using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.RemoveAuction;

public record RemoveAuctionCommand(Guid Id) : IRequest<Unit>, ITransactional;
