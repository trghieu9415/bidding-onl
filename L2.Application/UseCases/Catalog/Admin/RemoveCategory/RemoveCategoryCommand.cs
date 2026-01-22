using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Catalog.Admin.RemoveCategory;

public record RemoveCategoryCommand(Guid Id) : ICommand<Unit>;