using FluentAssertions;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Repositories.Read;
using L2.Application.UseCases.Transactions.Queries.GetBidderOrder;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Transactions.Queries;

public class GetBidderOrderHandlerTests {
  private readonly IOrderReadRepository _orderReadRepository = Substitute.For<IOrderReadRepository>();
  private readonly GetBidderOrderHandler _sut;

  public GetBidderOrderHandlerTests() {
    _sut = new GetBidderOrderHandler(_orderReadRepository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_OrderNotFoundOrNotOwned() {
    var request = new GetBidderOrderQuery(Guid.NewGuid(), Guid.NewGuid());

    _orderReadRepository.GetOrderPaymentByIdAsync(request.Id, CancellationToken.None)
      .Returns(((OrderDto?)null, new List<PaymentDto>()));

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Không tìm thấy đơn hàng");
  }

  [Fact]
  public async Task Handle_Should_ReturnOrderAndPayments_When_OwnerMatches() {
    var userId = Guid.NewGuid();
    var order = UseCaseTestData.CreateOrderDto(bidderId: userId);
    var payments = new List<PaymentDto> { UseCaseTestData.CreatePaymentDto(orderId: order.Id) };
    var request = new GetBidderOrderQuery(order.Id, userId);

    _orderReadRepository.GetOrderPaymentByIdAsync(request.Id, CancellationToken.None)
      .Returns((order, payments));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Order.Should().Be(order);
    result.Payments.Should().BeSameAs(payments);
  }
}
