using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.SyncAuctions;

public record SyncAuctionsCommand(Guid Id, List<Guid> AuctionIds) : ICommand<Unit>;