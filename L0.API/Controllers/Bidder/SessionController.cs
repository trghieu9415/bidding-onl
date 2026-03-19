using L0.API.Response;
using L2.Application.UseCases.Bidding.Bidder.GetSessions;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Bidder;

public class SessionController : UserController {
  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel) {
    var result = await Mediator.Send(new GetSessionsQuery(sieveModel));
    return AppResponse.Success(result.Sessions, result.Meta);
  }
}
