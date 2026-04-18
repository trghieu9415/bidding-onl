using L0.API.Response;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.DTOs;
using L2.Application.UseCases.Transactions.Commands.CreateOrder;
using L2.Application.UseCases.Transactions.Commands.CreatePayment;
using L2.Application.UseCases.Transactions.Commands.RefundPayment;
using L2.Application.UseCases.Transactions.Commands.VerifyPayment;
using L2.Application.UseCases.Transactions.Queries.GetBidderOrder;
using L2.Application.UseCases.Transactions.Queries.GetOrderHistory;
using L2.Application.UseCases.Transactions.Queries.GetOrders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Sieve.Models;

namespace L0.API.Controllers.Bidder;

[EnableRateLimiting("CheckoutPolicy")]
public class TransactionController : UserController {
  [HttpPost]
  [ProducesSuccess<List<IdData>>]
  public async Task<IActionResult> CreateOrder(
    [FromBody] CreateOrderRequest req,
    CancellationToken ct
  ) {
    var command = new CreateOrderCommand(CurrentUser.Id, CurrentUser.FullName, CurrentUser.Email, req);
    var result = await Mediator.Send(command, ct);
    return ApiResponse.Success(result.Id);
  }

  [HttpPost("{id:guid}/pay")]
  [ProducesSuccess<List<CreatePaymentResult>>]
  public async Task<IActionResult> CreatePayment(
    Guid orderId,
    [FromQuery] PaymentMethod method,
    CancellationToken ct
  ) {
    var command = new CreatePaymentCommand(orderId, CurrentUser.Id, method);
    var result = await Mediator.Send(command, ct);
    return ApiResponse.Success(result);
  }

  [HttpPost("verify")]
  [ProducesSuccess<List<bool>>]
  public async Task<IActionResult> VerifyPayment(
    [FromBody] VerifyPaymentRequest req,
    CancellationToken ct
  ) {
    var command = new VerifyPaymentCommand(CurrentUser.Id, req);
    var result = await Mediator.Send(command, ct);
    return ApiResponse.Success(result);
  }

  [HttpPost("payment/{id:guid}/refund")]
  [ProducesSuccess<List<bool>>]
  public async Task<IActionResult> RefundPayment(
    Guid id,
    CancellationToken ct
  ) {
    var command = new RefundPaymentCommand(id, CurrentUser.Id);
    var result = await Mediator.Send(command, ct);
    return ApiResponse.Success(result);
  }

  [HttpGet("history")]
  [ProducesSuccess<List<OrderDto>>]
  public async Task<IActionResult> GetOrders([FromBody] SieveModel sieveModel, CancellationToken ct) {
    var query = new GetOrderHistoryQuery(CurrentUser.Id, sieveModel);
    var result = await Mediator.Send(query, ct);
    return ApiResponse.Success(result);
  }

  [HttpGet("{id:guid}")]
  [ProducesSuccess<List<OrderDto>>]
  public async Task<IActionResult> GetOrders(Guid id, CancellationToken ct) {
    var query = new GetBidderOrderQuery(id, CurrentUser.Id);
    var result = await Mediator.Send(query, ct);
    return ApiResponse.Success(result);
  }
}
