using L0.API.Response;
using L2.Application.UseCases.Bidder.Admin.GetBidder;
using L2.Application.UseCases.Bidder.Admin.GetBidders;
using L2.Application.UseCases.Bidder.Admin.GetLockedBidders;
using L2.Application.UseCases.Bidder.Admin.LockBidder;
using L2.Application.UseCases.Bidder.Admin.UnlockBider;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Admin;

public class BidderController(IMediator mediator) : DashboardController {
  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel) {
    var result = await mediator.Send(new GetBiddersQuery(sieveModel));
    return AppResponse.Success(result.Bidders, result.Meta);
  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetById(Guid id) {
    var result = await mediator.Send(new GetBidderQuery(id));
    return AppResponse.Success(result.Bidder);
  }

  [HttpGet("locked")]
  public async Task<IActionResult> GetLocked([FromQuery] SieveModel sieveModel) {
    var result = await mediator.Send(new GetLockedBiddersQuery(sieveModel));
    return AppResponse.Success(result.Bidders, result.Meta);
  }

  [HttpPost("{id:guid}/lock")]
  public async Task<IActionResult> Lock(Guid id) {
    await mediator.Send(new LockBidderCommand(id));
    return AppResponse.Success("Đã khóa tài khoản người dùng");
  }

  [HttpPost("{id:guid}/unlock")]
  public async Task<IActionResult> Unlock(Guid id) {
    await mediator.Send(new UnlockBidderCommand(id));
    return AppResponse.Success("Đã mở khóa tài khoản người dùng");
  }
}
