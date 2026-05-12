using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.System.AssignWinner;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.System.Commands;

public class AssignWinnerHandlerTests {
  private readonly IRepository<CatalogItem> _repository = Substitute.For<IRepository<CatalogItem>>();
  private readonly AssignWinnerHandler _sut;

  public AssignWinnerHandlerTests() {
    _sut = new AssignWinnerHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_ItemNotFound() {
    var request = new AssignWinnerCommand(Guid.NewGuid(), true);

    _repository.GetByIdAsync(request.CatalogItemId, CancellationToken.None).Returns((CatalogItem?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Sản phẩm không tồn tại trong hệ thống");
  }

  [Fact]
  public async Task Handle_Should_MarkItemAsSoldOrUnsold_And_ReturnTrue() {
    var item = new CatalogItemBuilder().Build();
    var request = new AssignWinnerCommand(item.Id, true);

    _repository.GetByIdAsync(request.CatalogItemId, CancellationToken.None).Returns(item);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    item.Status.Should().Be(ItemStatus.Sold);
    await _repository.Received(1).UpdateAsync(item, CancellationToken.None);
  }
}
