using L0.API.Response;
using L2.Application.DTOs;
using L2.Application.UseCases.Categories.Queries.GetCategories;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Bidder;

public class CategoryController : UserController {
  [HttpGet]
  [ProducesSuccess<List<CategoryDto>>]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetCategoriesQuery(sieveModel), ct);
    return ApiResponse.Success(result.Categories, result.Meta);
  }
}
