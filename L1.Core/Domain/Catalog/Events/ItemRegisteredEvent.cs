using L1.Core.Base.Event;

namespace L1.Core.Domain.Catalog.Events;

public record ItemRegisteredEvent(
  Guid ItemId,
  Guid OwnerId,
  string Name
) : DomainEvent {
  public ItemRegisteredEvent() : this(Guid.Empty, Guid.Empty, "") {}
  public override Guid AggregateId => ItemId;
}
