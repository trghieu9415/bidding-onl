using L0.API.Response;
using L2.Application.UseCases.Catalog.Bidder.GetRegisteredItems;
using L2.Application.UseCases.Catalog.Bidder.RegisterItem;
using L2.Application.UseCases.Catalog.Bidder.SearchItem;
using L2.Application.UseCases.Catalog.Bidder.UpdateRegisteredItem;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Bidder;

public class CatalogItemController : UserController {
  [HttpGet("my-items")]
  public async Task<IActionResult> GetMyItems([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetRegisteredItemsQuery(sieveModel), ct);
    return AppResponse.Success(result.Items, result.Meta);
  }

  [HttpPost("register")]
  public async Task<IActionResult> RegisterItem([FromBody] RegisterItemCommand command, CancellationToken ct) {
    var id = await Mediator.Send(command, ct);
    return AppResponse.Success(id, "Sản phẩm đã được gửi, vui lòng chờ Admin phê duyệt");
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> UpdateItem(Guid id, [FromBody] UpdateRegisteredItemCommand command,
    CancellationToken ct) {
    command = command with { Id = id };
    await Mediator.Send(command, ct);
    return AppResponse.Success("Cập nhật thông tin sản phẩm thành công");
  }

  [HttpGet("search")]
  public async Task<IActionResult> Search([FromQuery] SearchItemQuery query, CancellationToken ct) {
    var result = await Mediator.Send(query, ct);
    return AppResponse.Success(result.Items, result.Meta);
  }
}
