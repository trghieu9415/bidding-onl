using L0.API.Response;
using L2.Application.UseCases.Categories.Commands.AddCategory;
using L2.Application.UseCases.Categories.Commands.RemoveCategory;
using L2.Application.UseCases.Categories.Commands.RestoreCategory;
using L2.Application.UseCases.Categories.Commands.UpdateCategory;
using L2.Application.UseCases.Categories.Queries.GetCategories;
using L2.Application.UseCases.Categories.Queries.GetCategory;
using L2.Application.UseCases.Categories.Queries.GetRemovedCategories;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Admin;

public class CategoryController : DashboardController {
  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetById(Guid id, CancellationToken ct) {
    var query = new GetCategoryQuery(id);
    var result = await Mediator.Send(query, ct);
    return AppResponse.Success(result.Category);
  }

  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetCategoriesQuery(sieveModel), ct);
    return AppResponse.Success(result.Categories, result.Meta);
  }

  [HttpGet("removed")]
  public async Task<IActionResult> GetRemoved([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetRemovedCategoriesQuery(sieveModel), ct);
    return AppResponse.Success(result.Categories, result.Meta);
  }

  [HttpPost]
  public async Task<IActionResult> Create(AddCategoryCommand command, CancellationToken ct) {
    var id = await Mediator.Send(command, ct);
    return AppResponse.Success(id, "Danh mục đã được tạo");
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest data, CancellationToken ct) {
    var command = new UpdateCategoryCommand(id, data);
    await Mediator.Send(command, ct);
    return AppResponse.Success("Danh mục đã được cập nhật");
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Remove(Guid id, CancellationToken ct) {
    await Mediator.Send(new RemoveCategoryCommand(id), ct);
    return AppResponse.Success("Danh mục đã được xóa");
  }

  [HttpPatch("{id:guid}/restore")]
  public async Task<IActionResult> Restore(Guid id, CancellationToken ct) {
    await Mediator.Send(new RestoreCategoryCommand(id), ct);
    return AppResponse.Success("Danh mục đã được khôi phục");
  }
}
