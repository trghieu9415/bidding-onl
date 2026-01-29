using L0.API.Response;
using L2.Application.UseCases.Bidding.Admin.AddAuction;
using L2.Application.UseCases.Bidding.Admin.GetAuction;
using L2.Application.UseCases.Bidding.Admin.GetAuctions;
using L2.Application.UseCases.Bidding.Admin.GetRemovedAuctions;
using L2.Application.UseCases.Bidding.Admin.RemoveAuction;
using L2.Application.UseCases.Bidding.Admin.RestoreAuction;
using L2.Application.UseCases.Bidding.Admin.UpdateAuction;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Admin;

public class AuctionController(IMediator mediator) : DashboardController {
  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel) {
    var result = await mediator.Send(new GetAuctionsQuery(sieveModel));
    return AppResponse.Success(result.Auctions, result.Meta);
  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetById(Guid id) {
    var result = await mediator.Send(new GetAuctionQuery(id));
    return AppResponse.Success(result.Auction);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] AddAuctionCommand command) {
    var id = await mediator.Send(command);
    return AppResponse.Success(id, "Tạo cuộc đấu giá thành công");
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAuctionCommand command) {
    command = command with { Id = id };
    await mediator.Send(command);
    return AppResponse.Success("Cập nhật thông tin đấu giá thành công");
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Delete(Guid id) {
    await mediator.Send(new RemoveAuctionCommand(id));
    return AppResponse.Success("Đã xóa cuộc đấu giá");
  }

  [HttpGet("removed")]
  public async Task<IActionResult> GetRemoved([FromQuery] SieveModel sieveModel) {
    var result = await mediator.Send(new GetRemovedAuctionsQuery(sieveModel));
    return AppResponse.Success(result.Auctions, result.Meta);
  }

  [HttpPatch("{id:guid}/restore")]
  public async Task<IActionResult> Restore(Guid id) {
    await mediator.Send(new RestoreAuctionCommand(id));
    return AppResponse.Success("Cuộc đấu giá đã được khôi phục");
  }
}
