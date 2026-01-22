using L2.Application.Abstractions;

namespace L2.Application.UseCases.Catalog.Admin.GetCategory;

public record GetCategoryQuery(Guid Id) : IQuery<GetCategoryResult>;