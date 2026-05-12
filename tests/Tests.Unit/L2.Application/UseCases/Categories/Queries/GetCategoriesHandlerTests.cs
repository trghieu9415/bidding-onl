using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Repositories;
using L2.Application.UseCases.Categories.Queries.GetCategories;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Categories.Queries;

public class GetCategoriesHandlerTests {
  private readonly IReadRepository<Category, CategoryDto> _readRepository =
    Substitute.For<IReadRepository<Category, CategoryDto>>();

  private readonly GetCategoriesHandler _sut;

  public GetCategoriesHandlerTests() {
    _sut = new GetCategoriesHandler(_readRepository);
  }

  [Fact]
  public async Task Handle_Should_ReturnCategories_And_Meta() {
    var filter = new CategoryFilter { Page = 1, PerPage = 10 };
    var categories = new List<CategoryDto> { UseCaseTestData.CreateCategoryDto() };
    var request = new GetCategoriesQuery(filter);

    _readRepository.GetAsync(filter: filter, ct: CancellationToken.None)
      .Returns((12, categories));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Categories.Should().BeSameAs(categories);
    result.Meta.Total.Should().Be(12);
    result.Meta.TotalPages.Should().Be(2);
  }
}
