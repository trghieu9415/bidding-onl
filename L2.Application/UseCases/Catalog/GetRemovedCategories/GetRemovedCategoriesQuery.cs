using L2.Application.DTOs;
using L2.Application.Models;
using MediatR;
using Sieve.Models;

namespace L2.Application.UseCases.Catalog.GetRemovedCategories;

public record GetRemovedCategoriesQuery(SieveModel SieveModel) : IRequest<GetRemovedCategoriesResult>;

public record GetRemovedCategoriesResult(List<CategoryDto> Categories, Meta Meta);
