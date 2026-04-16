using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bids.Commands.PlaceBid;

public record PlaceBidCommand(Guid AuctionId, PlaceBidRequest Data) : IRequest<Guid>, ITransactional, ILockable {
  public string LockKey => $"locks:auction:{AuctionId}";
  public TimeSpan WaitTime => TimeSpan.FromSeconds(5);
}

public record PlaceBidRequest(decimal Amount);
