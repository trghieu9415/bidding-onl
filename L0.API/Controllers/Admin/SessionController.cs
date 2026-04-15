using L0.API.Response;
using L2.Application.UseCases.Auctions.Commands.SyncAuctions;
using L2.Application.UseCases.Sessions.Commands.AddSession;
using L2.Application.UseCases.Sessions.Commands.PublishSession;
using L2.Application.UseCases.Sessions.Commands.RemoveSession;
using L2.Application.UseCases.Sessions.Commands.RestoreSession;
using L2.Application.UseCases.Sessions.Commands.UpdateSession;
using L2.Application.UseCases.Sessions.Queries.GetRemovedSessions;
using L2.Application.UseCases.Sessions.Queries.GetSessions;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Admin;

public class SessionController : DashboardController {
  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetSessionsQuery(sieveModel), ct);
    return AppResponse.Success(result.Sessions, result.Meta);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] AddSessionCommand command, CancellationToken ct) {
    var id = await Mediator.Send(command, ct);
    return AppResponse.Success(id, "Tạo phiên đấu giá thành công");
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSessionCommand command, CancellationToken ct) {
    command = command with { Id = id };
    await Mediator.Send(command, ct);
    return AppResponse.Success("Cập nhật phiên đấu giá thành công");
  }

  [HttpPost("{id:guid}/sync")]
  public async Task<IActionResult> SyncAuctions(Guid id, [FromBody] List<Guid> auctionIds, CancellationToken ct) {
    await Mediator.Send(new SyncAuctionsCommand(id, auctionIds), ct);
    return AppResponse.Success("Đã cập nhật danh sách đấu giá vào phiên");
  }

  [HttpPatch("{id:guid}/publish")]
  public async Task<IActionResult> Publish(Guid id, CancellationToken ct) {
    await Mediator.Send(new PublishSessionCommand(id), ct);
    return AppResponse.Success("Phiên đấu giá đã được công khai");
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Delete(Guid id, CancellationToken ct) {
    await Mediator.Send(new RemoveSessionCommand(id), ct);
    return AppResponse.Success("Đã xóa phiên đấu giá");
  }

  [HttpGet("removed")]
  public async Task<IActionResult> GetRemoved([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetRemovedSessionsQuery(sieveModel), ct);
    return AppResponse.Success(result.Sessions, result.Meta);
  }

  [HttpPatch("{id:guid}/restore")]
  public async Task<IActionResult> Restore(Guid id, CancellationToken ct) {
    await Mediator.Send(new RestoreSessionCommand(id), ct);
    return AppResponse.Success("Phiên đấu giá đã được khôi phục");
  }
}
