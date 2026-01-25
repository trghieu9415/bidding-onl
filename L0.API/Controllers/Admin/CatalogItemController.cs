using L0.API.Response;
using L2.Application.UseCases.Catalog.Admin.ApproveItem;
using L2.Application.UseCases.Catalog.Admin.GetItem;
using L2.Application.UseCases.Catalog.Admin.GetItems;
using L2.Application.UseCases.Catalog.Admin.RejectItem;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Admin;

public class CatalogItemController(IMediator mediator) : DashboardController {
  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel) {
    var result = await mediator.Send(new GetItemsQuery(sieveModel));
    return AppResponse.Success(result.Items, result.Meta);
  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetById(Guid id) {
    var result = await mediator.Send(new GetItemQuery(id));
    return AppResponse.Success(result.Item);
  }

  [HttpPatch("{id:guid}/approve")]
  public async Task<IActionResult> Approve(Guid id) {
    await mediator.Send(new ApproveItemCommand(id));
    return AppResponse.Success("Sản phẩm đã được phê duyệt");
  }

  [HttpPatch("{id:guid}/reject")]
  public async Task<IActionResult> Reject(Guid id, [FromBody] RejectItemCommand command) {
    command = command with { Id = id };
    await mediator.Send(command);
    return AppResponse.Success("Sản phẩm đã bị từ chối");
  }
}
