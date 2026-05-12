using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.Items.Queries.GetItem;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Items.Queries;

public class GetItemHandlerTests {
  private readonly IReadRepository<CatalogItem, CatalogItemDto> _readRepository =
    Substitute.For<IReadRepository<CatalogItem, CatalogItemDto>>();

  private readonly GetItemHandler _sut;

  public GetItemHandlerTests() {
    _sut = new GetItemHandler(_readRepository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_ItemNotFound() {
    var request = new GetItemQuery(Guid.NewGuid());

    _readRepository.GetByIdAsync(request.Id, CancellationToken.None).Returns((CatalogItemDto?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Sản phẩm không tồn tại");
  }

  [Fact]
  public async Task Handle_Should_ReturnItem_When_Found() {
    var item = UseCaseTestData.CreateCatalogItemDto();
    var request = new GetItemQuery(item.Id);

    _readRepository.GetByIdAsync(request.Id, CancellationToken.None).Returns(item);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Item.Should().Be(item);
  }
}
