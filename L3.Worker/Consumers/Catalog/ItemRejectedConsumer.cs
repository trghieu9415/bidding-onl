using L1.Core.Base.Event;

namespace L3.Worker.Consumers.Catalog;

public record ItemRejectedConsumer(Guid ItemId, Guid OwnerId, string Reason = "") : DomainEvent {
  public override Guid AggregateId => ItemId;
}
