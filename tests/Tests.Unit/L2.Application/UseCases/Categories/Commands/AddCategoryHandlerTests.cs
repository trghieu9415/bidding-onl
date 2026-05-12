using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.Repositories;
using L2.Application.UseCases.Categories.Commands.AddCategory;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Categories.Commands;

public class AddCategoryHandlerTests {
  private readonly IRepository<Category> _repository = Substitute.For<IRepository<Category>>();
  private readonly AddCategoryHandler _sut;

  public AddCategoryHandlerTests() {
    _sut = new AddCategoryHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_CreateCategory_And_ReturnId() {
    var request = new AddCategoryCommand("Điện tử", Guid.NewGuid());
    var createdId = Guid.NewGuid();

    _repository.CreateAsync(Arg.Any<Category>(), CancellationToken.None).Returns(createdId);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().Be(createdId);
    await _repository.Received(1).CreateAsync(
      Arg.Is<Category>(x => x.Name == request.Name && x.ParentId == request.ParentId),
      CancellationToken.None
    );
  }
}
