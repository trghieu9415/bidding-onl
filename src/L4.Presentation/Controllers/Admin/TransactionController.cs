using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.UseCases.Transactions.Queries.GetOrder;
using L2.Application.UseCases.Transactions.Queries.GetOrders;
using L4.Presentation.Response;
using Microsoft.AspNetCore.Mvc;

namespace L4.Presentation.Controllers.Admin;

public class TransactionController : DashboardController {
  [HttpGet]
  [ProducesSuccess<List<OrderDto>>]
  public async Task<IActionResult> Get([FromQuery] OrderFilter filter, CancellationToken ct) {
    var result = await Mediator.Send(new GetOrdersQuery(filter), ct);
    return ApiResponse.Success(result.Orders, result.Meta);
  }

  [HttpGet("{id:guid}")]
  [ProducesSuccess<OrderDto>]
  public async Task<IActionResult> GetDetail(Guid id, CancellationToken ct) {
    var result = await Mediator.Send(new GetOrderQuery(id), ct);
    return ApiResponse.Success(result.Order);
  }
}
