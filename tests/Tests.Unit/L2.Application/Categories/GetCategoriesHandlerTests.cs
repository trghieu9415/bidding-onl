using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using L2.Application.UseCases.Categories.Queries.GetCategories;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Categories;

public class GetCategoriesHandlerTests {
  [Fact]
  public async Task Handle_ReturnsEntitiesAndMeta() {
    var categories = new List<CategoryDto> { new() { Id = Guid.NewGuid(), Name = "Electronics" } };
    var filter = new CategoryFilter { Page = 2, PerPage = 5 };
    var readRepo = new StubReadRepository<Category, CategoryDto> { GetAsyncResult = (8, categories) };
    var handler = new GetCategoriesHandler(readRepo);

    var result = await handler.Handle(new GetCategoriesQuery(filter), TestContext.Current.CancellationToken);

    Assert.Equal(categories, result.Categories);
    Assert.Equal(Meta.Create(2, 5, 8), result.Meta);
  }
}
