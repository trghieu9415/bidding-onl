using L0.API.Response;
using L2.Application.UseCases.Bidding.Bidder.GetSessions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Bidder;

public class SessionController(IMediator mediator) : UserController {
  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel) {
    var result = await mediator.Send(new GetSessionsQuery(sieveModel));
    return AppResponse.Success(result.Sessions, result.Meta);
  }
}
