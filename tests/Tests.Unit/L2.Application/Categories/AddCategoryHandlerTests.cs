using L1.Core.Domain.Catalog.Entities;
using L2.Application.UseCases.Categories.Commands.AddCategory;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Categories;

public class AddCategoryHandlerTests {
  [Fact]
  public async Task Handle_CreatesCategoryAndReturnsId() {
    var repo = new StubRepository<Category> { CreateResult = Guid.NewGuid() };
    var handler = new AddCategoryHandler(repo);

    var result = await handler.Handle(
      new AddCategoryCommand("Electronics", null),
      TestContext.Current.CancellationToken
    );

    Assert.Equal(repo.CreateResult, result);
    var createdCategory = Assert.IsType<Category>(repo.CreatedEntity);
    Assert.Equal("Electronics", createdCategory.Name);
    Assert.Null(createdCategory.ParentId);
  }
}
