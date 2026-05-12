using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.Repositories;
using L2.Application.UseCases.Categories.Commands.RestoreCategory;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Categories.Commands;

public class RestoreCategoryHandlerTests {
  private readonly IRepository<Category> _repository = Substitute.For<IRepository<Category>>();
  private readonly RestoreCategoryHandler _sut;

  public RestoreCategoryHandlerTests() {
    _sut = new RestoreCategoryHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_RestoreCategory_And_ReturnTrue() {
    var request = new RestoreCategoryCommand(Guid.NewGuid());

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    await _repository.Received(1).RestoreAsync(request.Id, CancellationToken.None);
  }
}
