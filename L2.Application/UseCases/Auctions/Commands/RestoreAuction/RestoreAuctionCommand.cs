using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auctions.Commands.RestoreAuction;

public record RestoreAuctionCommand(Guid Id) : IRequest<Unit>, ITransactional;
