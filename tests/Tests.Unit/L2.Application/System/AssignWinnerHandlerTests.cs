using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.UseCases.System.AssignWinner;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.System;

public class AssignWinnerHandlerTests {
  [Fact]
  public async Task Handle_WhenItemFound_UpdatesSoldStatus() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");
    var repo = new StubRepository<CatalogItem> { EntityByIdResult = item };
    var handler = new AssignWinnerHandler(repo);

    var result = await handler.Handle(
      new AssignWinnerCommand(item.Id, true),
      TestContext.Current.CancellationToken
    );

    Assert.True(result);
    Assert.Same(item, repo.UpdatedEntity);
    Assert.Equal(ItemStatus.Sold, item.Status);
  }
}
