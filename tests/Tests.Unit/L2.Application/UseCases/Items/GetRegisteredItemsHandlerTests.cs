using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using L2.Application.UseCases.Items.Queries.GetRegisteredItems;
using Tests.Unit.L2.Application.UseCases.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Items;

public class GetRegisteredItemsHandlerTests {
  [Fact]
  public async Task Handle_AppliesOwnerCriteriaAndReturnsMeta() {
    var ownerId = Guid.NewGuid();
    var items = new List<CatalogItemDto> {
      new() { Id = Guid.NewGuid(), OwnerId = ownerId, Name = "Laptop", Description = "Gaming laptop" }
    };
    var filter = new CatalogItemFilter { Page = 1, PerPage = 10 };
    var readRepo = new StubReadRepository<CatalogItem, CatalogItemDto> { GetAsyncResult = (4, items) };
    var handler = new GetRegisteredItemsHandler(readRepo);

    var result = await handler.Handle(
      new GetRegisteredItemsQuery(ownerId, filter),
      TestContext.Current.CancellationToken
    );

    Assert.Equal(items, result.Items);
    Assert.Equal(Meta.Create(1, 10, 4), result.Meta);
    Assert.NotNull(readRepo.LastCriteria);

    var criteria = readRepo.LastCriteria!;
    Assert.True(criteria.Compile()(CatalogItem.Create(ownerId, "Laptop", "Gaming laptop")));
    Assert.False(criteria.Compile()(CatalogItem.Create(Guid.NewGuid(), "Phone", "Smart phone")));
  }
}
