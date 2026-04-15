using System.Text.Json.Serialization;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bids.Commands.PlaceBid;

public record PlaceBidCommand(decimal Amount) : IRequest<Guid>, ITransactional, ILockable {
  [JsonIgnore] public Guid AuctionId { get; init; }
  public string LockKey => $"locks:auction:{AuctionId}";
  public TimeSpan WaitTime => TimeSpan.FromSeconds(5);
}
