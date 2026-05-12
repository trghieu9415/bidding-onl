using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Repositories;
using L2.Application.UseCases.Items.Commands.RegisterItem;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Items.Commands;

public class RegisterItemHandlerTests {
  private readonly IRepository<CatalogItem> _repository = Substitute.For<IRepository<CatalogItem>>();
  private readonly RegisterItemHandler _sut;

  public RegisterItemHandlerTests() {
    _sut = new RegisterItemHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_CreateItem_And_ReturnId() {
    var userId = Guid.NewGuid();
    var categoryId = Guid.NewGuid();
    var request = new RegisterItemCommand(
      userId,
      new RegisterItemRequest(
        "Laptop",
        "Gaming laptop",
        1000,
        ItemCondition.NewSealed,
        [categoryId],
        "https://example.com/main.jpg",
        ["https://example.com/sub.jpg"]
      )
    );
    var createdId = Guid.NewGuid();

    _repository.CreateAsync(Arg.Any<CatalogItem>(), CancellationToken.None).Returns(createdId);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().Be(createdId);
    await _repository.Received(1).CreateAsync(
      Arg.Is<CatalogItem>(x =>
        x.OwnerId == userId &&
        x.Name == request.Data.Name &&
        x.Description == request.Data.Description &&
        x.StartingPrice == request.Data.StartingPrice &&
        x.Condition == request.Data.Condition &&
        x.CategoryIds.Contains(categoryId) &&
        x.Images.MainImageUrl == request.Data.MainImageUrl
      ),
      CancellationToken.None
    );
  }
}
