using L2.Application.Abstractions;
using Sieve.Models;

namespace L2.Application.UseCases.Catalog.Admin.GetRemovedCategories;

public record GetRemovedCategoriesQuery(SieveModel SieveModel) : IQuery<GetRemovedCategoriesResult>;