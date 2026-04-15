using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Categories.Commands.RestoreCategory;

public record RestoreCategoryCommand(Guid Id) : IRequest<Unit>, ITransactional;
