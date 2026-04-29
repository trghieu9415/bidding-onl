using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.UseCases.Items.Commands.RevokeItem;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Items;

public class RevokeItemHandlerTests {
  [Fact]
  public async Task Handle_WhenItemBelongsToUser_RevokesAndPersists() {
    var ownerId = Guid.NewGuid();
    var item = CatalogItem.Create(ownerId, "Laptop", "Gaming laptop");
    var repo = new StubRepository<CatalogItem> { EntityByIdResult = item };
    var handler = new RevokeItemHandler(repo);

    var result = await handler.Handle(new RevokeItemCommand(item.Id, ownerId), TestContext.Current.CancellationToken);

    Assert.True(result);
    Assert.Same(item, repo.UpdatedEntity);
    Assert.Equal(ItemStatus.Revoked, item.Status);
  }
}
