using L0.API.Response;
using L2.Application.Models;
using L2.Application.UseCases.Bidders.Commands.LockBidder;
using L2.Application.UseCases.Bidders.Commands.UnlockBidder;
using L2.Application.UseCases.Bidders.Queries.GetBidder;
using L2.Application.UseCases.Bidders.Queries.GetBidders;
using L2.Application.UseCases.Bidders.Queries.GetLockedBidders;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Admin;

public class BidderController : DashboardController {
  [HttpGet]
  [ProducesSuccess<List<User>>]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetBiddersQuery(sieveModel), ct);
    return ApiResponse.Success(result.Bidders, result.Meta);
  }

  [HttpGet("{id:guid}")]
  [ProducesSuccess<User>]
  public async Task<IActionResult> GetById(Guid id, CancellationToken ct) {
    var result = await Mediator.Send(new GetBidderQuery(id), ct);
    return ApiResponse.Success(result.Bidder);
  }

  [HttpGet("locked")]
  [ProducesSuccess<List<User>>]
  public async Task<IActionResult> GetLocked([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetLockedBiddersQuery(sieveModel), ct);
    return ApiResponse.Success(result.Bidders, result.Meta);
  }

  [HttpPost("{id:guid}/lock")]
  [ProducesSuccess<bool>]
  public async Task<IActionResult> Lock(Guid id, CancellationToken ct) {
    var result = await Mediator.Send(new LockBidderCommand(id), ct);
    return ApiResponse.Success(result, "Đã khóa tài khoản người dùng");
  }

  [HttpPost("{id:guid}/unlock")]
  [ProducesSuccess<bool>]
  public async Task<IActionResult> Unlock(Guid id, CancellationToken ct) {
    var result = await Mediator.Send(new UnlockBidderCommand(id), ct);
    return ApiResponse.Success(result, "Đã mở khóa tài khoản người dùng");
  }
}
