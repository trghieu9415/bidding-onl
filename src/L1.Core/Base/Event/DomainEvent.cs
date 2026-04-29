namespace L1.Core.Base.Event;

public abstract record DomainEvent {
  public Guid Id { get; init; } = Guid.NewGuid();
  public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
  public abstract Guid AggregateId { get; }
}
