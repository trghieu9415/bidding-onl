using L2.Application.DTOs;

namespace L2.Application.UseCases.Catalog.Admin.GetCategories;

public record GetCategoriesResult(List<CategoryDto> Category, int Total);