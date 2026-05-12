using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Repositories;
using L2.Application.UseCases.Items.Queries.GetRemovedItems;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Items.Queries;

public class GetRemovedItemsHandlerTests {
  private readonly IReadRepository<CatalogItem, CatalogItemDto> _readRepository =
    Substitute.For<IReadRepository<CatalogItem, CatalogItemDto>>();

  private readonly GetRemovedItemsHandler _sut;

  public GetRemovedItemsHandlerTests() {
    _sut = new GetRemovedItemsHandler(_readRepository);
  }

  [Fact]
  public async Task Handle_Should_ReturnRemovedItems_And_Meta() {
    var filter = new CatalogItemFilter { Page = 2, PerPage = 10 };
    var items = new List<CatalogItemDto> { UseCaseTestData.CreateCatalogItemDto() };
    var request = new GetRemovedItemsQuery(filter);

    _readRepository.GetDeletedAsync(filter: filter, ct: CancellationToken.None).Returns((30, items));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Items.Should().BeSameAs(items);
    result.Meta.Page.Should().Be(2);
    result.Meta.TotalPages.Should().Be(3);
  }
}
