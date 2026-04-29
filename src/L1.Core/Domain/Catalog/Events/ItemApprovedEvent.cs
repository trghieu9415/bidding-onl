using L1.Core.Base.Event;

namespace L1.Core.Domain.Catalog.Events;

public record ItemApprovedEvent(
  Guid ItemId,
  Guid OwnerId
) : DomainEvent {
  public override Guid AggregateId => ItemId;
}
