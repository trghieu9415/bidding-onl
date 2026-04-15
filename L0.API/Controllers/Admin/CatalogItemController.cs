using L0.API.Response;
using L2.Application.UseCases.Items.Commands.ApproveItem;
using L2.Application.UseCases.Items.Commands.RejectItem;
using L2.Application.UseCases.Items.Queries.GetItem;
using L2.Application.UseCases.Items.Queries.GetItems;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Admin;

// [Authorize]
public class CatalogItemController : DashboardController {
  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetItemsQuery(sieveModel), ct);
    return AppResponse.Success(result.Items, result.Meta);
  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetById(Guid id, CancellationToken ct) {
    var result = await Mediator.Send(new GetItemQuery(id), ct);
    return AppResponse.Success(result.Item);
  }

  [HttpPatch("{id:guid}/approve")]
  public async Task<IActionResult> Approve(Guid id, CancellationToken ct) {
    await Mediator.Send(new ApproveItemCommand(id), ct);
    return AppResponse.Success("Sản phẩm đã được phê duyệt");
  }

  [HttpPatch("{id:guid}/reject")]
  public async Task<IActionResult> Reject(Guid id, [FromBody] RejectItemCommand command, CancellationToken ct) {
    command = command with { Id = id };
    await Mediator.Send(command, ct);
    return AppResponse.Success("Sản phẩm đã bị từ chối");
  }
}
