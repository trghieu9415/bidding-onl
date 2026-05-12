using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.Categories.Queries.GetCategory;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Categories.Queries;

public class GetCategoryHandlerTests {
  private readonly IReadRepository<Category, CategoryDto> _readRepository =
    Substitute.For<IReadRepository<Category, CategoryDto>>();

  private readonly GetCategoryHandler _sut;

  public GetCategoryHandlerTests() {
    _sut = new GetCategoryHandler(_readRepository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_CategoryNotFound() {
    var request = new GetCategoryQuery(Guid.NewGuid());

    _readRepository.GetByIdAsync(request.Id, CancellationToken.None)
      .Returns((CategoryDto?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Danh mục không tồn tại");
  }

  [Fact]
  public async Task Handle_Should_ReturnCategory_When_Found() {
    var category = UseCaseTestData.CreateCategoryDto();
    var request = new GetCategoryQuery(category.Id);

    _readRepository.GetByIdAsync(request.Id, CancellationToken.None)
      .Returns(category);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Category.Should().Be(category);
  }
}
