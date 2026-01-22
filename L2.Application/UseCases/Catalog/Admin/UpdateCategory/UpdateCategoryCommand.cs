using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Catalog.Admin.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, string Name, Guid? ParentId) : ICommand<Unit>;