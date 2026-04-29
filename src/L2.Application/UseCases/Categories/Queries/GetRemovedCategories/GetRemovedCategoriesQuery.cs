using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Categories.Queries.GetRemovedCategories;

public record GetRemovedCategoriesQuery(CategoryFilter Filter) : IRequest<GetRemovedCategoriesResult>;

public record GetRemovedCategoriesResult(List<CategoryDto> Categories, Meta Meta);
