using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Exceptions;
using L2.Application.UseCases.Items.Commands.UpdateRegisteredItem;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Items;

public class UpdateRegisteredItemHandlerTests {
  [Fact]
  public async Task Handle_WhenUserIsNotOwner_ThrowsWorkflowException() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");
    var repo = new StubRepository<CatalogItem> { EntityByIdResult = item };
    var handler = new UpdateRegisteredItemHandler(repo);

    var exception = await Assert.ThrowsAsync<WorkflowException>(async () =>
      await handler.Handle(new UpdateRegisteredItemCommand(item.Id, Guid.NewGuid(), new UpdateRegisteredItemRequest("Phone", null, null, null, null, null, null)), TestContext.Current.CancellationToken));

    Assert.Equal(403, exception.StatusCode);
    Assert.Equal("Bạn không có quyền chỉnh sửa sản phẩm này", exception.Message);
  }

  [Fact]
  public async Task Handle_WhenValid_UpdatesOnlyProvidedFields() {
    var ownerId = Guid.NewGuid();
    var item = CatalogItem.Create(ownerId, "Laptop", "Gaming laptop")
      .SetStartingPrice(150m)
      .SetCondition(ItemCondition.NewSealed)
      .SyncCategories([Guid.NewGuid()])
      .SetImageUrls("main.png", ["sub-1.png"]);
    var repo = new StubRepository<CatalogItem> { EntityByIdResult = item };
    var request = new UpdateRegisteredItemRequest(
      "Ultrabook",
      null,
      220m,
      ItemCondition.OpenBox,
      [Guid.NewGuid(), Guid.NewGuid()],
      null,
      ["sub-2.png"]
    );
    var handler = new UpdateRegisteredItemHandler(repo);

    var result = await handler.Handle(new UpdateRegisteredItemCommand(item.Id, ownerId, request), default);

    Assert.True(result);
    Assert.Same(item, repo.UpdatedEntity);
    Assert.Equal("Ultrabook", item.Name);
    Assert.Equal("Gaming laptop", item.Description);
    Assert.Equal(220m, item.StartingPrice);
    Assert.Equal(ItemCondition.OpenBox, item.Condition);
    Assert.Equal(request.CategoryIds, item.CategoryIds);
    Assert.Equal("main.png", item.Images.MainImageUrl);
    Assert.Equal(["sub-2.png"], item.Images.SubImageUrls);
  }
}
