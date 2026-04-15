using L2.Application.Abstractions;
using L2.Application.DTOs;

namespace L2.Application.UseCases.Catalog.GetCategory;

public record GetCategoryQuery(Guid Id) : IQuery<GetCategoryResult>;

public record GetCategoryResult(CategoryDto Category);
