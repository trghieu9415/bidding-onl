using L2.Application.DTOs;

namespace L2.Application.UseCases.Catalog.Admin.GetRemovedCategories;

public record GetRemovedCategoriesResult(List<CategoryDto> Categories, int Total);