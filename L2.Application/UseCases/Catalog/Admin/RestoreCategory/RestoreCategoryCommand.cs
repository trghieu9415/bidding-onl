using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Catalog.Admin.RestoreCategory;

public record RestoreCategoryCommand(Guid Id) : ICommand<Unit>;
