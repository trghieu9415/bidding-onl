using L1.Core.Base.Event;

namespace L3.Worker.Consumers.Catalog;

public record ItemRegisteredConsumer(Guid ItemId, Guid OwnerId, string Name) : DomainEvent {
  public override Guid AggregateId => ItemId;
}
