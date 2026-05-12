using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Repositories;
using L2.Application.UseCases.Categories.Queries.GetRemovedCategories;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Categories.Queries;

public class GetRemovedCategoriesHandlerTests {
  private readonly IReadRepository<Category, CategoryDto> _readRepository =
    Substitute.For<IReadRepository<Category, CategoryDto>>();

  private readonly GetRemovedCategoriesHandler _sut;

  public GetRemovedCategoriesHandlerTests() {
    _sut = new GetRemovedCategoriesHandler(_readRepository);
  }

  [Fact]
  public async Task Handle_Should_ReturnRemovedCategories_And_Meta() {
    var filter = new CategoryFilter { Page = 2, PerPage = 10 };
    var categories = new List<CategoryDto> { UseCaseTestData.CreateCategoryDto() };
    var request = new GetRemovedCategoriesQuery(filter);

    _readRepository.GetDeletedAsync(filter: filter, ct: CancellationToken.None)
      .Returns((25, categories));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Categories.Should().BeSameAs(categories);
    result.Meta.Page.Should().Be(2);
    result.Meta.TotalPages.Should().Be(3);
  }
}
