using FluentAssertions;
using L1.Core.Domain.Catalog.Events;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.Events;

public class CatalogEventTests {
  [Fact]
  public void Events_ExposeExpectedAggregateIds() {
    // Arrange
    var itemId = Guid.NewGuid();

    // Act
    var approvedEvent = new ItemApprovedEvent(itemId, Guid.NewGuid());
    var registeredEvent = new ItemRegisteredEvent(itemId, Guid.NewGuid(), "Laptop");
    var rejectedEvent = new ItemRejectedEvent(itemId, Guid.NewGuid(), "Reason");

    // Assert
    approvedEvent.AggregateId.Should().Be(itemId);
    registeredEvent.AggregateId.Should().Be(itemId);
    rejectedEvent.AggregateId.Should().Be(itemId);
  }
}
