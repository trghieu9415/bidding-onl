using FluentAssertions;
using L1.Core.Base.Event;
using Xunit;

namespace Tests.Unit.L1.Core.Base.Event;

public class DomainEventTests {
  [Fact]
  public void NewDomainEvent_InitializesIdOccurredOnAndAggregateId() {
    // Arrange
    var aggregateId = Guid.NewGuid();
    var before = DateTime.UtcNow;

    // Act
    var domainEvent = new TestDomainEvent(aggregateId);

    // Assert
    var after = DateTime.UtcNow;

    domainEvent.Id.Should().NotBeEmpty();
    domainEvent.AggregateId.Should().Be(aggregateId);
    domainEvent.OccurredOn
      .Should().BeOnOrAfter(before)
      .And.BeOnOrBefore(after);
  }

  private sealed record TestDomainEvent(Guid EntityId) : DomainEvent {
    public override Guid AggregateId => EntityId;
  }
}
