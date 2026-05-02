using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.UseCases.Items.Commands.ApproveItem;
using Tests.Unit.L2.Application.UseCases.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Items;

public class ApproveItemHandlerTests {
  [Fact]
  public async Task Handle_WhenFound_ApprovesAndPersists() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");
    var repo = new StubRepository<CatalogItem> { EntityByIdResult = item };
    var handler = new ApproveItemHandler(repo);

    var result = await handler.Handle(
      new ApproveItemCommand(item.Id),
      TestContext.Current.CancellationToken
    );

    Assert.True(result);
    Assert.Same(item, repo.UpdatedEntity);
    Assert.Equal(ItemStatus.Approval, item.Status);
  }
}
