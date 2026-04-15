using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.SyncAuctions;

public record SyncAuctionsCommand(Guid Id, List<Guid> AuctionIds) : IRequest<Unit>, ITransactional;
