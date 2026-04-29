using L1.Core.Base.Event;
using Xunit;

namespace Tests.Unit.L1.Core.Base;

public class DomainEventTests {
  [Fact]
  public void NewDomainEvent_InitializesIdOccurredOnAndAggregateId() {
    var aggregateId = Guid.NewGuid();
    var before = DateTime.UtcNow;

    var domainEvent = new TestDomainEvent(aggregateId);

    var after = DateTime.UtcNow;
    Assert.NotEqual(Guid.Empty, domainEvent.Id);
    Assert.Equal(aggregateId, domainEvent.AggregateId);
    Assert.InRange(domainEvent.OccurredOn, before, after);
  }

  private sealed record TestDomainEvent(Guid EntityId) : DomainEvent {
    public override Guid AggregateId => EntityId;
  }
}
