using L1.Core.Base.Event;

namespace L1.Core.Domain.Catalog.Events;

public record ItemRejectedEvent(Guid ItemId, Guid OwnerId, string Reason = "") : DomainEvent {
  public override Guid AggregateId => ItemId;
}
