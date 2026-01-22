using L2.Application.DTOs;
using L2.Application.Models;

namespace L2.Application.UseCases.Catalog.Admin.GetCategories;

public record GetCategoriesResult(List<CategoryDto> Categories, Meta Meta);
