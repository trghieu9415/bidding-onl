using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.RestoreAuction;

public record RestoreAuctionCommand(Guid Id) : IRequest<Unit>, ITransactional;
