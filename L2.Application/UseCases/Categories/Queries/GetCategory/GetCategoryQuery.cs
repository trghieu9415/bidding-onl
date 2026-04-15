using L2.Application.DTOs;
using MediatR;

namespace L2.Application.UseCases.Categories.Queries.GetCategory;

public record GetCategoryQuery(Guid Id) : IRequest<GetCategoryResult>;

public record GetCategoryResult(CategoryDto Category);
