using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.Categories.Commands.UpdateCategory;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Categories.Commands;

public class UpdateCategoryHandlerTests {
  private readonly IRepository<Category> _repository = Substitute.For<IRepository<Category>>();
  private readonly UpdateCategoryHandler _sut;

  public UpdateCategoryHandlerTests() {
    _sut = new UpdateCategoryHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_CategoryNotFound() {
    var request = new UpdateCategoryCommand(Guid.NewGuid(), new UpdateCategoryRequest("Name", null));

    _repository.GetByIdAsync(request.Id, CancellationToken.None)
      .Returns((Category?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Không tìm thấy danh mục");
  }

  [Fact]
  public async Task Handle_Should_UpdateCategory_And_ReturnTrue() {
    var category = Category.Create("Old Name");
    var parentId = Guid.NewGuid();
    var request = new UpdateCategoryCommand(category.Id, new UpdateCategoryRequest("New Name", parentId));

    _repository.GetByIdAsync(request.Id, CancellationToken.None)
      .Returns(category);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    category.Name.Should().Be("New Name");
    category.ParentId.Should().Be(parentId);
    await _repository.Received(1).UpdateAsync(category, CancellationToken.None);
  }
}
