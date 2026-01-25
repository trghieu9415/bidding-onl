using L1.Core.Base.Event;

namespace L3.Worker.Consumers.Catalog;

public record ItemApprovedConsumer(Guid ItemId, Guid OwnerId) : DomainEvent {
  public override Guid AggregateId => ItemId;
}
