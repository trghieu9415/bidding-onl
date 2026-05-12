using System.Linq.Expressions;
using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Repositories;
using L2.Application.UseCases.Items.Queries.GetRegisteredItems;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Items.Queries;

public class GetRegisteredItemsHandlerTests {
  private readonly IReadRepository<CatalogItem, CatalogItemDto> _readRepository =
    Substitute.For<IReadRepository<CatalogItem, CatalogItemDto>>();

  private readonly GetRegisteredItemsHandler _sut;

  public GetRegisteredItemsHandlerTests() {
    _sut = new GetRegisteredItemsHandler(_readRepository);
  }

  [Fact]
  public async Task Handle_Should_Filter_By_Owner_And_ReturnMeta() {
    var userId = Guid.NewGuid();
    var filter = new CatalogItemFilter { Page = 1, PerPage = 10 };
    var items = new List<CatalogItemDto> { UseCaseTestData.CreateCatalogItemDto(ownerId: userId) };
    var request = new GetRegisteredItemsQuery(userId, filter);
    Expression<Func<CatalogItem, bool>>? capturedCriteria = null;

    _readRepository.GetAsync(
        Arg.Do<Expression<Func<CatalogItem, bool>>?>(x => capturedCriteria = x),
        filter,
        CancellationToken.None
      )
      .Returns((8, items));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Items.Should().BeSameAs(items);
    result.Meta.Total.Should().Be(8);
    capturedCriteria.Should().NotBeNull();

    var predicate = capturedCriteria!.Compile();
    predicate(new Tests.Common.Builders.CatalogItemBuilder().WithOwnerId(userId).Build()).Should().BeTrue();
    predicate(new Tests.Common.Builders.CatalogItemBuilder().WithOwnerId(Guid.NewGuid()).Build()).Should().BeFalse();
  }
}
