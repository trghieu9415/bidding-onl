using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.UseCases.Categories.Queries.GetCategory;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Categories;

public class GetCategoryHandlerTests {
  [Fact]
  public async Task Handle_WhenFound_ReturnsCategory() {
    var dto = new CategoryDto { Id = Guid.NewGuid(), Name = "Electronics" };
    var readRepo = new StubReadRepository<Category, CategoryDto> { EntityByIdResult = dto };
    var handler = new GetCategoryHandler(readRepo);

    var result = await handler.Handle(
      new GetCategoryQuery(dto.Id),
      TestContext.Current.CancellationToken
    );

    Assert.Same(dto, result.Category);
  }
}
