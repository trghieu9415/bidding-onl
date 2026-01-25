using L1.Core.Base.Event;

namespace L1.Core.Domain.Bidding.Events;

public record SessionPublishedEvent(Guid SessionId, string Title) : DomainEvent {
  public override Guid AggregateId => SessionId;
}
