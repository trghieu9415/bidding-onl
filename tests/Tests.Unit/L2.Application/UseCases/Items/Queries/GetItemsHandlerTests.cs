using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Repositories;
using L2.Application.UseCases.Items.Queries.GetItems;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Items.Queries;

public class GetItemsHandlerTests {
  private readonly IReadRepository<CatalogItem, CatalogItemDto> _readRepository =
    Substitute.For<IReadRepository<CatalogItem, CatalogItemDto>>();

  private readonly GetItemsHandler _sut;

  public GetItemsHandlerTests() {
    _sut = new GetItemsHandler(_readRepository);
  }

  [Fact]
  public async Task Handle_Should_ReturnItems_And_Meta() {
    var filter = new CatalogItemFilter { Page = 1, PerPage = 10 };
    var items = new List<CatalogItemDto> { UseCaseTestData.CreateCatalogItemDto() };
    var request = new GetItemsQuery(filter);

    _readRepository.GetAsync(filter: filter, ct: CancellationToken.None).Returns((13, items));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Items.Should().BeSameAs(items);
    result.Meta.Total.Should().Be(13);
    result.Meta.TotalPages.Should().Be(2);
  }
}
