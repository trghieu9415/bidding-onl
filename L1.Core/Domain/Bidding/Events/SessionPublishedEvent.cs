using L1.Core.Base.Event;

namespace L1.Core.Domain.Bidding.Events;

public record SessionPublishedEvent(
  Guid SessionId,
  string Title,
  DateTime StartTime,
  DateTime EndTime
) : DomainEvent {
  public SessionPublishedEvent() : this(Guid.Empty, "", DateTime.MinValue, DateTime.MinValue) {}
  public override Guid AggregateId => SessionId;
}
