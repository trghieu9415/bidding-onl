using L0.API.Response;
using L2.Application.DTOs;
using L2.Application.UseCases.Sessions.Queries.GetSessions;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Admin;

public class TransactionController : DashboardController {
  [HttpGet]
  [ProducesSuccess<List<AuctionSessionDto>>]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetSessionsQuery(sieveModel), ct);
    return ApiResponse.Success(result.Sessions, result.Meta);
  }
}
