using System.Linq.Expressions;
using FluentAssertions;
using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Repositories;
using L2.Application.UseCases.Transactions.Queries.GetOrderHistory;
using NSubstitute;
using Tests.Common.Builders;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Transactions.Queries;

public class GetOrderHistoryHandlerTests {
  private readonly IReadRepository<Order, OrderDto> _readRepository = Substitute.For<IReadRepository<Order, OrderDto>>();
  private readonly GetOrderHistoryHandler _sut;

  public GetOrderHistoryHandlerTests() {
    _sut = new GetOrderHistoryHandler(_readRepository);
  }

  [Fact]
  public async Task Handle_Should_Filter_By_Bidder_And_ReturnMeta() {
    var userId = Guid.NewGuid();
    var filter = new OrderFilter { Page = 1, PerPage = 10 };
    var orders = new List<OrderDto> { UseCaseTestData.CreateOrderDto(bidderId: userId) };
    var request = new GetOrderHistoryQuery(userId, filter);
    Expression<Func<Order, bool>>? capturedCriteria = null;

    _readRepository.GetAsync(
        Arg.Do<Expression<Func<Order, bool>>?>(x => capturedCriteria = x),
        filter,
        CancellationToken.None
      )
      .Returns((4, orders));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Orders.Should().BeSameAs(orders);
    result.Meta.Total.Should().Be(4);
    capturedCriteria.Should().NotBeNull();

    var predicate = capturedCriteria!.Compile();
    predicate(new OrderBuilder().WithBidderId(userId).Build()).Should().BeTrue();
    predicate(new OrderBuilder().WithBidderId(Guid.NewGuid()).Build()).Should().BeFalse();
  }
}
