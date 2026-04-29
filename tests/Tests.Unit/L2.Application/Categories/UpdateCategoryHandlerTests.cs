using L1.Core.Domain.Catalog.Entities;
using L2.Application.Exceptions;
using L2.Application.UseCases.Categories.Commands.UpdateCategory;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Categories;

public class UpdateCategoryHandlerTests {
  [Fact]
  public async Task Handle_WhenCategoryMissing_ThrowsWorkflowException() {
    var handler = new UpdateCategoryHandler(new StubRepository<Category>());

    var exception = await Assert.ThrowsAsync<WorkflowException>(async () =>
      await handler.Handle(new UpdateCategoryCommand(Guid.NewGuid(), new UpdateCategoryRequest("Phones", null)), TestContext.Current.CancellationToken));

    Assert.Equal(404, exception.StatusCode);
    Assert.Equal("Không tìm thấy danh mục", exception.Message);
  }

  [Fact]
  public async Task Handle_WhenCategoryExists_UpdatesAndPersists() {
    var category = Category.Create("Electronics");
    var repo = new StubRepository<Category> { EntityByIdResult = category };
    var parentId = Guid.NewGuid();
    var handler = new UpdateCategoryHandler(repo);

    var result = await handler.Handle(new UpdateCategoryCommand(category.Id, new UpdateCategoryRequest("Phones", parentId)), TestContext.Current.CancellationToken);

    Assert.True(result);
    Assert.Same(category, repo.UpdatedEntity);
    Assert.Equal("Phones", category.Name);
    Assert.Equal(parentId, category.ParentId);
  }
}
