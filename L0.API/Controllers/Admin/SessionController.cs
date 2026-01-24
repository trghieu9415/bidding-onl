using L0.API.Response;
using L2.Application.UseCases.Bidding.Admin.AddSession;
using L2.Application.UseCases.Bidding.Admin.GetRemovedSessions;
using L2.Application.UseCases.Bidding.Admin.GetSessions;
using L2.Application.UseCases.Bidding.Admin.PublishSession;
using L2.Application.UseCases.Bidding.Admin.RemoveSession;
using L2.Application.UseCases.Bidding.Admin.RestoreSession;
using L2.Application.UseCases.Bidding.Admin.SyncAuctions;
using L2.Application.UseCases.Bidding.Admin.UpdateSession;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Admin;

public class SessionController(IMediator mediator) : DashboardController {
  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel) {
    var result = await mediator.Send(new GetSessionsQuery(sieveModel));
    return AppResponse.Success(result.Sessions, result.Meta);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] AddSessionCommand command) {
    var id = await mediator.Send(command);
    return AppResponse.Success(id, "Tạo phiên đấu giá thành công");
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSessionCommand command) {
    command = command with { Id = id };
    await mediator.Send(command);
    return AppResponse.Success("Cập nhật phiên đấu giá thành công");
  }

  [HttpPost("{id:guid}/sync")]
  public async Task<IActionResult> SyncAuctions(Guid id, [FromBody] List<Guid> auctionIds) {
    await mediator.Send(new SyncAuctionsCommand(id, auctionIds));
    return AppResponse.Success("Đã cập nhật danh sách đấu giá vào phiên");
  }

  [HttpPatch("{id:guid}/publish")]
  public async Task<IActionResult> Publish(Guid id) {
    await mediator.Send(new PublishSessionCommand(id));
    return AppResponse.Success("Phiên đấu giá đã được công khai");
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Delete(Guid id) {
    await mediator.Send(new RemoveSessionCommand(id));
    return AppResponse.Success("Đã xóa phiên đấu giá");
  }

  [HttpGet("removed")]
  public async Task<IActionResult> GetRemoved([FromQuery] SieveModel sieveModel) {
    var result = await mediator.Send(new GetRemovedSessionsQuery(sieveModel));
    return AppResponse.Success(result.Sessions, result.Meta);
  }

  [HttpPatch("{id:guid}/restore")]
  public async Task<IActionResult> Restore(Guid id) {
    await mediator.Send(new RestoreSessionCommand(id));
    return AppResponse.Success("Phiên đấu giá đã được khôi phục");
  }
}
