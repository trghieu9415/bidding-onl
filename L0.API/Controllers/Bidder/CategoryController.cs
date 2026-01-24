using L0.API.Response;
using L2.Application.UseCases.Catalog.Bidder.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Bidder;

public class CategoryController(IMediator mediator) : UserController {
  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel) {
    var result = await mediator.Send(new GetCategoriesQuery(sieveModel));
    return AppResponse.Success(result.Category, result.Meta);
  }
}
