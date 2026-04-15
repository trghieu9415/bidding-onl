using L0.API.Response;
using L2.Application.UseCases.Bidding.AddAuction;
using L2.Application.UseCases.Bidding.GetAuction;
using L2.Application.UseCases.Bidding.GetAuctions;
using L2.Application.UseCases.Bidding.GetRemovedAuctions;
using L2.Application.UseCases.Bidding.RemoveAuction;
using L2.Application.UseCases.Bidding.RestoreAuction;
using L2.Application.UseCases.Bidding.UpdateAuction;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Admin;

public class AuctionController : DashboardController {
  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetAuctionsQuery(sieveModel), ct);
    return AppResponse.Success(result.Auctions, result.Meta);
  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetById(Guid id, CancellationToken ct) {
    var result = await Mediator.Send(new GetAuctionQuery(id), ct);
    return AppResponse.Success(result.Auction);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] AddAuctionCommand command, CancellationToken ct) {
    var id = await Mediator.Send(command, ct);
    return AppResponse.Success(id, "Tạo cuộc đấu giá thành công");
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAuctionCommand command, CancellationToken ct) {
    command = command with { Id = id };
    await Mediator.Send(command, ct);
    return AppResponse.Success("Cập nhật thông tin đấu giá thành công");
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Delete(Guid id, CancellationToken ct) {
    await Mediator.Send(new RemoveAuctionCommand(id), ct);
    return AppResponse.Success("Đã xóa cuộc đấu giá");
  }

  [HttpGet("removed")]
  public async Task<IActionResult> GetRemoved([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetRemovedAuctionsQuery(sieveModel), ct);
    return AppResponse.Success(result.Auctions, result.Meta);
  }

  [HttpPatch("{id:guid}/restore")]
  public async Task<IActionResult> Restore(Guid id, CancellationToken ct) {
    await Mediator.Send(new RestoreAuctionCommand(id), ct);
    return AppResponse.Success("Cuộc đấu giá đã được khôi phục");
  }
}
