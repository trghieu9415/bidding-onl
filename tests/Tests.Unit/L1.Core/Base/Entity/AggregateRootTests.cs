using System.Diagnostics.CodeAnalysis;
using L1.Core.Base.Entity;
using L1.Core.Base.Event;
using Xunit;

namespace Tests.Unit.L1.Core.Base.Entity;

public class AggregateRootTests {
  [Fact]
  public void AddDomainEvent_AddsEventToReadOnlyCollection() {
    var aggregate = new TestAggregateRoot();
    var domainEvent = new TestDomainEvent(aggregate.Id);

    aggregate.AddDomainEvent(domainEvent);

    var storedEvent = Assert.Single(aggregate.DomainEvents);
    Assert.Same(domainEvent, storedEvent);
  }

  [Fact]
  public void ClearEvents_RemovesAllDomainEvents() {
    var aggregate = new TestAggregateRoot();
    aggregate.AddDomainEvent(new TestDomainEvent(aggregate.Id));
    aggregate.AddDomainEvent(new TestDomainEvent(aggregate.Id));

    aggregate.ClearEvents();

    Assert.Empty(aggregate.DomainEvents);
  }

  private sealed class TestAggregateRoot : AggregateRoot;

  [ExcludeFromCodeCoverage]
  private sealed record TestDomainEvent(Guid EntityId) : DomainEvent {
    public override Guid AggregateId => EntityId;
  }
}
