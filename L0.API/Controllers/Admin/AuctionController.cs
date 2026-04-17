using L0.API.Response;
using L2.Application.DTOs;
using L2.Application.UseCases.Auctions.Commands.AddAuction;
using L2.Application.UseCases.Auctions.Commands.RemoveAuction;
using L2.Application.UseCases.Auctions.Commands.RestoreAuction;
using L2.Application.UseCases.Auctions.Commands.UpdateAuction;
using L2.Application.UseCases.Auctions.Queries.GetAuction;
using L2.Application.UseCases.Auctions.Queries.GetAuctions;
using L2.Application.UseCases.Auctions.Queries.GetRemovedAuctions;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Admin;

public class AuctionController : DashboardController {
  [HttpGet]
  [ProducesSuccess<List<AuctionDto>>]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetAuctionsQuery(sieveModel), ct);
    return ApiResponse.Success(result.Auctions, result.Meta);
  }

  [HttpGet("{id:guid}")]
  [ProducesSuccess<AuctionDto>]
  public async Task<IActionResult> GetById(Guid id, CancellationToken ct) {
    var result = await Mediator.Send(new GetAuctionQuery(id), ct);
    return ApiResponse.Success(result.Auction);
  }

  [HttpPost]
  [ProducesSuccess<IdData>]
  public async Task<IActionResult> Create([FromBody] AddAuctionCommand command, CancellationToken ct) {
    var id = await Mediator.Send(command, ct);
    return ApiResponse.Success(id, "Tạo cuộc đấu giá thành công");
  }

  [HttpPut("{id:guid}")]
  [ProducesSuccess<bool>]
  public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAuctionCommand command, CancellationToken ct) {
    command = command with { Id = id };
    var result = await Mediator.Send(command, ct);
    return ApiResponse.Success(result, "Cập nhật thông tin đấu giá thành công");
  }

  [HttpDelete("{id:guid}")]
  [ProducesSuccess<bool>]
  public async Task<IActionResult> Delete(Guid id, CancellationToken ct) {
    var result = await Mediator.Send(new RemoveAuctionCommand(id), ct);
    return ApiResponse.Success(result, "Đã xóa cuộc đấu giá");
  }

  [HttpGet("removed")]
  [ProducesSuccess<List<AuctionDto>>]
  public async Task<IActionResult> GetRemoved([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetRemovedAuctionsQuery(sieveModel), ct);
    return ApiResponse.Success(result.Auctions, result.Meta);
  }

  [HttpPatch("{id:guid}/restore")]
  [ProducesSuccess<bool>]
  public async Task<IActionResult> Restore(Guid id, CancellationToken ct) {
    var result = await Mediator.Send(new RestoreAuctionCommand(id), ct);
    return ApiResponse.Success(result, "Cuộc đấu giá đã được khôi phục");
  }
}
