using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, UpdateCategoryRequest Data) : IRequest<Unit>, ITransactional;

public record UpdateCategoryRequest(string Name, Guid? ParentId);
