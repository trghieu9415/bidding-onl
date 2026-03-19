using L0.API.Response;
using L2.Application.UseCases.Catalog.Admin.AddCategory;
using L2.Application.UseCases.Catalog.Admin.GetCategories;
using L2.Application.UseCases.Catalog.Admin.GetCategory;
using L2.Application.UseCases.Catalog.Admin.GetRemovedCategories;
using L2.Application.UseCases.Catalog.Admin.RemoveCategory;
using L2.Application.UseCases.Catalog.Admin.RestoreCategory;
using L2.Application.UseCases.Catalog.Admin.UpdateCategory;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Admin;

public class CategoryController : DashboardController {
  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetById(Guid id) {
    var query = new GetCategoryQuery(id);
    var result = await Mediator.Send(query);
    return AppResponse.Success(result.Category);
  }

  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel) {
    var result = await Mediator.Send(new GetCategoriesQuery(sieveModel));
    return AppResponse.Success(result.Categories, result.Meta);
  }

  [HttpGet("removed")]
  public async Task<IActionResult> GetRemoved([FromQuery] SieveModel sieveModel) {
    var result = await Mediator.Send(new GetRemovedCategoriesQuery(sieveModel));
    return AppResponse.Success(result.Categories, result.Meta);
  }

  [HttpPost]
  public async Task<IActionResult> Create(AddCategoryCommand command) {
    var id = await Mediator.Send(command);
    return AppResponse.Success(id, "Danh mục đã được tạo");
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command) {
    command = command with { Id = id };
    await Mediator.Send(command);
    return AppResponse.Success("Danh mục đã được cập nhật");
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Remove(Guid id) {
    await Mediator.Send(new RemoveCategoryCommand(id));
    return AppResponse.Success("Danh mục đã được xóa");
  }

  [HttpPatch("{id:guid}/restore")]
  public async Task<IActionResult> Restore(Guid id) {
    await Mediator.Send(new RestoreCategoryCommand(id));
    return AppResponse.Success("Danh mục đã được khôi phục");
  }
}
