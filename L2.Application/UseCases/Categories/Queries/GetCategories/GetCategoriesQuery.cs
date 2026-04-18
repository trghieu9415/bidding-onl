using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Categories.Queries.GetCategories;

public record GetCategoriesQuery(CategoryFilter Filter) : IRequest<GetCategoriesResult>;

public record GetCategoriesResult(List<CategoryDto> Categories, Meta Meta);
