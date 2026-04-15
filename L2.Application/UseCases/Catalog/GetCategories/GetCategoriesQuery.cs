using L2.Application.Abstractions;
using L2.Application.DTOs;
using L2.Application.Models;
using Sieve.Models;

namespace L2.Application.UseCases.Catalog.GetCategories;

public record GetCategoriesQuery(SieveModel SieveModel) : IQuery<GetCategoriesResult>;

public record GetCategoriesResult(List<CategoryDto> Categories, Meta Meta);
