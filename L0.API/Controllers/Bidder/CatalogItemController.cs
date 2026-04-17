using L0.API.Response;
using L2.Application.DTOs;
using L2.Application.UseCases.Items.Commands.RegisterItem;
using L2.Application.UseCases.Items.Commands.UpdateRegisteredItem;
using L2.Application.UseCases.Items.Queries.GetRegisteredItems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Sieve.Models;

namespace L0.API.Controllers.Bidder;

public class CatalogItemController : UserController {
  [HttpGet("my-items")]
  [ProducesSuccess<List<CatalogItemDto>>]
  public async Task<IActionResult> GetMyItems([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetRegisteredItemsQuery(sieveModel), ct);
    return ApiResponse.Success(result.Items, result.Meta);
  }

  [HttpPost("register")]
  [ProducesSuccess<IdData>]
  public async Task<IActionResult> RegisterItem([FromBody] RegisterItemCommand command, CancellationToken ct) {
    var id = await Mediator.Send(command, ct);
    return ApiResponse.Success(id, "Sản phẩm đã được gửi, vui lòng chờ Admin phê duyệt");
  }

  [HttpPut("{id:guid}")]
  [ProducesSuccess<bool>]
  [EnableRateLimiting("CheckoutPolicy")]
  public async Task<IActionResult> UpdateItem(
    Guid id, [FromBody] UpdateRegisteredItemRequest req,
    CancellationToken ct
  ) {
    var command = new UpdateRegisteredItemCommand(id, req);
    var result = await Mediator.Send(command, ct);
    return ApiResponse.Success(result, "Cập nhật thông tin sản phẩm thành công");
  }
}
