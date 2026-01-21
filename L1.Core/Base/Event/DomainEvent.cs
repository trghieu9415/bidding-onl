namespace L1.Core.Base.Event;

public abstract record DomainEvent {
  public Guid Id { get; init; } = Guid.NewGuid();
  public DateTime OccurredOn { get; init; } = DateTime.Now;
  public Guid AggregateId { get; init; }
}
