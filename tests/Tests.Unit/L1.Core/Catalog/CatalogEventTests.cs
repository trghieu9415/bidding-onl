using L1.Core.Domain.Catalog.Events;
using Xunit;

namespace Tests.Unit.L1.Core.Catalog;

public class CatalogEventTests {
  [Fact]
  public void Events_ExposeExpectedAggregateIds() {
    var itemId = Guid.NewGuid();

    var approvedEvent = new ItemApprovedEvent(itemId, Guid.NewGuid());
    var registeredEvent = new ItemRegisteredEvent(itemId, Guid.NewGuid(), "Laptop");
    var rejectedEvent = new ItemRejectedEvent(itemId, Guid.NewGuid(), "Reason");

    Assert.Equal(itemId, approvedEvent.AggregateId);
    Assert.Equal(itemId, registeredEvent.AggregateId);
    Assert.Equal(itemId, rejectedEvent.AggregateId);
  }
}
