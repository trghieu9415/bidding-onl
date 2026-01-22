using L2.Application.Abstractions;
using Sieve.Models;

namespace L2.Application.UseCases.Catalog.Admin.GetCategories;

public record GetCategoriesQuery(SieveModel SieveModel) : IQuery<GetCategoriesResult>;