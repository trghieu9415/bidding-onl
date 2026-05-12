using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.Repositories;
using L2.Application.UseCases.Categories.Commands.RemoveCategory;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Categories.Commands;

public class RemoveCategoryHandlerTests {
  private readonly IRepository<Category> _repository = Substitute.For<IRepository<Category>>();
  private readonly RemoveCategoryHandler _sut;

  public RemoveCategoryHandlerTests() {
    _sut = new RemoveCategoryHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_DeleteCategory_And_ReturnTrue() {
    var request = new RemoveCategoryCommand(Guid.NewGuid());

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    await _repository.Received(1).DeleteAsync(request.Id, CancellationToken.None);
  }
}
