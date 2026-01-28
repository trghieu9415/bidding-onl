using L1.Core.Base.Event;

namespace L1.Core.Domain.Catalog.Events;

public record ItemApprovedEvent(
  Guid ItemId,
  Guid OwnerId
) : DomainEvent {
  public ItemApprovedEvent() : this(Guid.Empty, Guid.Empty) {}
  public override Guid AggregateId => ItemId;
}
