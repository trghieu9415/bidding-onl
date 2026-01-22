using L2.Application.DTOs;
using L2.Application.Models;

namespace L2.Application.UseCases.Catalog.Admin.GetRemovedCategories;

public record GetRemovedCategoriesResult(List<CategoryDto> Categories, Meta Meta);