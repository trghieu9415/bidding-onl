using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.Repositories;
using L2.Application.UseCases.Items.Commands.RestoreItem;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Items.Commands;

public class RestoreItemHandlerTests {
  private readonly IRepository<CatalogItem> _repository = Substitute.For<IRepository<CatalogItem>>();
  private readonly RestoreItemHandler _sut;

  public RestoreItemHandlerTests() {
    _sut = new RestoreItemHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_RestoreItem_And_ReturnTrue() {
    var request = new RestoreItemCommand(Guid.NewGuid());

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    await _repository.Received(1).RestoreAsync(request.Id, CancellationToken.None);
  }
}
