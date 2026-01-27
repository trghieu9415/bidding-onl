using L0.API.Response;
using L2.Application.UseCases.Catalog.Bidder.GetRegisteredItems;
using L2.Application.UseCases.Catalog.Bidder.RegisterItem;
using L2.Application.UseCases.Catalog.Bidder.SearchItem;
using L2.Application.UseCases.Catalog.Bidder.UpdateRegisteredItem;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Bidder;

public class CatalogItemController(IMediator mediator) : UserController {
  [HttpGet("my-items")]
  public async Task<IActionResult> GetMyItems([FromQuery] SieveModel sieveModel) {
    var result = await mediator.Send(new GetRegisteredItemsQuery(sieveModel));
    return AppResponse.Success(result.Items, result.Meta);
  }

  [HttpPost("register")]
  public async Task<IActionResult> RegisterItem([FromBody] RegisterItemCommand command) {
    var id = await mediator.Send(command);
    return AppResponse.Success(id, "Sản phẩm đã được gửi, vui lòng chờ Admin phê duyệt");
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> UpdateItem(Guid id, [FromBody] UpdateRegisteredItemCommand command) {
    command = command with { Id = id };
    await mediator.Send(command);
    return AppResponse.Success("Cập nhật thông tin sản phẩm thành công");
  }

  [HttpGet("search")]
  public async Task<IActionResult> Search([FromQuery] SearchItemQuery query) {
    var result = await mediator.Send(query);
    return AppResponse.Success(result.Items, result.Meta);
  }
}
