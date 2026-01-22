using L2.Application.DTOs;

namespace L2.Application.UseCases.Catalog.Bidder.GetCategories;

public record GetCategoriesResult(List<CategoryDto> Category, int Total);