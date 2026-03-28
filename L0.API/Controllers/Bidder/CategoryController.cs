using L0.API.Response;
using L2.Application.UseCases.Catalog.Bidder.GetCategories;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Bidder;

public class CategoryController : UserController {
  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetCategoriesQuery(sieveModel), ct);
    return AppResponse.Success(result.Category, result.Meta);
  }
}
