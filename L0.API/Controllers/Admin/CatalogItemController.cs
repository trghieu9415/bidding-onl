using L0.API.Response;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.UseCases.Items.Commands.ApproveItem;
using L2.Application.UseCases.Items.Commands.RejectItem;
using L2.Application.UseCases.Items.Queries.GetItem;
using L2.Application.UseCases.Items.Queries.GetItems;
using L2.Application.UseCases.Items.Queries.GetRemovedItems;
using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers.Admin;

public class CatalogItemController : DashboardController {
  [HttpGet]
  [ProducesSuccess<List<CatalogItemDto>>]
  public async Task<IActionResult> Get([FromQuery] CatalogItemFilter filter, CancellationToken ct) {
    var result = await Mediator.Send(new GetItemsQuery(filter), ct);
    return ApiResponse.Success(result.Items, result.Meta);
  }

  [HttpGet("removed")]
  [ProducesSuccess<List<CatalogItemDto>>]
  public async Task<IActionResult> GetRemoved([FromQuery] CatalogItemFilter filter, CancellationToken ct) {
    var result = await Mediator.Send(new GetRemovedItemsQuery(filter), ct);
    return ApiResponse.Success(result.Items, result.Meta);
  }

  [HttpGet("{id:guid}")]
  [ProducesSuccess<CatalogItemDto>]
  public async Task<IActionResult> GetById(Guid id, CancellationToken ct) {
    var result = await Mediator.Send(new GetItemQuery(id), ct);
    return ApiResponse.Success(result.Item);
  }

  [HttpPatch("{id:guid}/approve")]
  [ProducesSuccess<bool>]
  public async Task<IActionResult> Approve(Guid id, CancellationToken ct) {
    var result = await Mediator.Send(new ApproveItemCommand(id), ct);
    return ApiResponse.Success(result, "Sản phẩm đã được phê duyệt");
  }

  [HttpPatch("{id:guid}/reject")]
  [ProducesSuccess<bool>]
  public async Task<IActionResult> Reject(Guid id, [FromBody] RejectItemRequest req, CancellationToken ct) {
    var command = new RejectItemCommand(id, req);
    var result = await Mediator.Send(command, ct);
    return ApiResponse.Success(result, "Sản phẩm đã bị từ chối");
  }
}
