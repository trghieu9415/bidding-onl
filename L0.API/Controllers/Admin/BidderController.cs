using L0.API.Response;
using L2.Application.UseCases.Bidder.GetBidder;
using L2.Application.UseCases.Bidder.GetBidders;
using L2.Application.UseCases.Bidder.GetLockedBidders;
using L2.Application.UseCases.Bidder.LockBidder;
using L2.Application.UseCases.Bidder.UnlockBidder;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Admin;

public class BidderController : DashboardController {
  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetBiddersQuery(sieveModel), ct);
    return AppResponse.Success(result.Bidders, result.Meta);
  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetById(Guid id, CancellationToken ct) {
    var result = await Mediator.Send(new GetBidderQuery(id), ct);
    return AppResponse.Success(result.Bidder);
  }

  [HttpGet("locked")]
  public async Task<IActionResult> GetLocked([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetLockedBiddersQuery(sieveModel), ct);
    return AppResponse.Success(result.Bidders, result.Meta);
  }

  [HttpPost("{id:guid}/lock")]
  public async Task<IActionResult> Lock(Guid id, CancellationToken ct) {
    await Mediator.Send(new LockBidderCommand(id), ct);
    return AppResponse.Success("Đã khóa tài khoản người dùng");
  }

  [HttpPost("{id:guid}/unlock")]
  public async Task<IActionResult> Unlock(Guid id, CancellationToken ct) {
    await Mediator.Send(new UnlockBidderCommand(id), ct);
    return AppResponse.Success("Đã mở khóa tài khoản người dùng");
  }
}
