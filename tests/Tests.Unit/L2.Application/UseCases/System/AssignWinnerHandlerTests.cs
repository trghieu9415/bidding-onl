using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Exceptions;
using L2.Application.UseCases.System.AssignWinner;
using Tests.Unit.L2.Application.UseCases.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.System;

public class AssignWinnerHandlerTests {
  [Fact]
  public async Task Handle_ItemNotFound_ThrowsWorkflowException() {
    var repo = new StubRepository<CatalogItem> { EntityByIdResult = null };
    var handler = new AssignWinnerHandler(repo);
    var command = new AssignWinnerCommand(Guid.NewGuid(), true);

    await Assert.ThrowsAsync<WorkflowException>(() =>
      handler.Handle(command, CancellationToken.None));
  }

  [Fact]
  public async Task Handle_WhenIsSoldIsTrue_UpdatesToSoldStatus() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming");
    var repo = new StubRepository<CatalogItem> { EntityByIdResult = item };
    var handler = new AssignWinnerHandler(repo);

    var result = await handler.Handle(
      new AssignWinnerCommand(item.Id, true),
      CancellationToken.None
    );

    Assert.True(result);
    Assert.Equal(ItemStatus.Sold, item.Status);
    Assert.Same(item, repo.UpdatedEntity);
  }

  [Fact]
  public async Task Handle_WhenIsSoldIsFalse_UpdatesToAvailableStatus() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming");
    var repo = new StubRepository<CatalogItem> { EntityByIdResult = item };
    var handler = new AssignWinnerHandler(repo);

    var result = await handler.Handle(
      new AssignWinnerCommand(item.Id, false),
      CancellationToken.None
    );

    Assert.True(result);
    Assert.Equal(ItemStatus.Unsold, item.Status);
    Assert.Same(item, repo.UpdatedEntity);
  }
}
