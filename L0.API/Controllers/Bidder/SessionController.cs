using L0.API.Response;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.UseCases.Sessions.Queries.GetSessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers.Bidder;

public class SessionController : UserController {
  [HttpGet]
  [ProducesSuccess<List<AuctionSessionDto>>]
  [AllowAnonymous]
  public async Task<IActionResult> Get([FromQuery] SessionFilter filter, CancellationToken ct) {
    var result = await Mediator.Send(new GetSessionsQuery(filter), ct);
    return ApiResponse.Success(result.Sessions, result.Meta);
  }
}
