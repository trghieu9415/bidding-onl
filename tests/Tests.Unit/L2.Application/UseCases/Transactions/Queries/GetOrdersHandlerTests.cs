using FluentAssertions;
using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Repositories;
using L2.Application.UseCases.Transactions.Queries.GetOrders;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Transactions.Queries;

public class GetOrdersHandlerTests {
  private readonly IReadRepository<Order, OrderDto> _readRepository = Substitute.For<IReadRepository<Order, OrderDto>>();
  private readonly GetOrdersHandler _sut;

  public GetOrdersHandlerTests() {
    _sut = new GetOrdersHandler(_readRepository);
  }

  [Fact]
  public async Task Handle_Should_ReturnOrders_And_Meta() {
    var filter = new OrderFilter { Page = 1, PerPage = 10 };
    var orders = new List<OrderDto> { UseCaseTestData.CreateOrderDto() };
    var request = new GetOrdersQuery(filter);

    _readRepository.GetAsync(filter: filter, ct: CancellationToken.None).Returns((16, orders));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Orders.Should().BeSameAs(orders);
    result.Meta.Total.Should().Be(16);
    result.Meta.TotalPages.Should().Be(2);
  }
}
