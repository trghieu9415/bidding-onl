using FluentAssertions;
using L1.Core.Base.Entity;
using L1.Core.Base.Event;
using Xunit;

namespace Tests.Unit.L1.Core.Base.Entity;

public class AggregateRootTests {
  [Fact]
  public void AddDomainEvent_AddsEventToReadOnlyCollection() {
    // Arrange
    var aggregate = new FakeAggregateRoot();
    var domainEvent = new FakeDomainEvent(aggregate.Id);

    // Act
    aggregate.AddDomainEvent(domainEvent);

    // Assert
    domainEvent.AggregateId.Should().Be(aggregate.Id);
    aggregate.DomainEvents.Should().ContainSingle()
      .Which.Should().BeSameAs(domainEvent);
  }

  [Fact]
  public void ClearEvents_RemovesAllDomainEvents() {
    // Arrange
    var aggregate = new FakeAggregateRoot();
    aggregate.AddDomainEvent(new FakeDomainEvent(aggregate.Id));
    aggregate.AddDomainEvent(new FakeDomainEvent(aggregate.Id));

    // Act
    aggregate.ClearEvents();

    // Assert
    aggregate.DomainEvents.Should().BeEmpty();
  }

  private sealed class FakeAggregateRoot : AggregateRoot;

  private sealed record FakeDomainEvent(Guid EntityId) : DomainEvent {
    public override Guid AggregateId => EntityId;
  }
}
