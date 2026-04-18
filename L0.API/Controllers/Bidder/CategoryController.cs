using L0.API.Response;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.UseCases.Categories.Queries.GetCategories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers.Bidder;

public class CategoryController : UserController {
  [HttpGet]
  [ProducesSuccess<List<CategoryDto>>]
  [AllowAnonymous]
  public async Task<IActionResult> Get([FromQuery] CategoryFilter filter, CancellationToken ct) {
    var result = await Mediator.Send(new GetCategoriesQuery(filter), ct);
    return ApiResponse.Success(result.Categories, result.Meta);
  }
}
