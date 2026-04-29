using L1.Core.Domain.Catalog.Entities;
using L2.Application.Exceptions;
using L2.Application.UseCases.Items.Commands.RejoinItem;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Items;

public class RejoinItemHandlerTests {
  [Fact]
  public async Task Handle_WhenItemDoesNotBelongToUser_ThrowsWorkflowException() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");
    var repo = new StubRepository<CatalogItem> { EntityByIdResult = item };
    var handler = new RejoinItemHandler(repo);

    var exception = await Assert.ThrowsAsync<WorkflowException>(async () =>
      await handler.Handle(new RejoinItemCommand(item.Id, Guid.NewGuid()), TestContext.Current.CancellationToken));

    Assert.Equal("Không tìm thấy sản phẩm", exception.Message);
  }
}
