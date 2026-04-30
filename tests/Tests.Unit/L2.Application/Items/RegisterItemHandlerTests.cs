using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.UseCases.Items.Commands.RegisterItem;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Items;

public class RegisterItemHandlerTests {
  [Fact]
  public async Task Handle_CreatesItemWithAllProvidedFields() {
    var repo = new StubRepository<CatalogItem> { CreateResult = Guid.NewGuid() };
    var request = new RegisterItemRequest(
      "Laptop",
      "Gaming laptop",
      150m,
      ItemCondition.OpenBox,
      [Guid.NewGuid()],
      "main.png",
      ["sub-1.png", "sub-2.png"]
    );
    var handler = new RegisterItemHandler(repo);

    var result = await handler.Handle(
      new RegisterItemCommand(Guid.NewGuid(), request),
      TestContext.Current.CancellationToken
    );

    Assert.Equal(repo.CreateResult, result);
    var createdItem = Assert.IsType<CatalogItem>(repo.CreatedEntity);
    Assert.Equal("Laptop", createdItem.Name);
    Assert.Equal("Gaming laptop", createdItem.Description);
    Assert.Equal(150m, createdItem.StartingPrice);
    Assert.Equal(ItemCondition.OpenBox, createdItem.Condition);
    Assert.Equal(request.CategoryIds, createdItem.CategoryIds);
    Assert.Equal("main.png", createdItem.Images.MainImageUrl);
    Assert.Equal(request.SubImageUrls, createdItem.Images.SubImageUrls);
  }
}
