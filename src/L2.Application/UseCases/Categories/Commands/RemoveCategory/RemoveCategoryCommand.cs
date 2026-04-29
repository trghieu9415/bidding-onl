using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Categories.Commands.RemoveCategory;

public record RemoveCategoryCommand(Guid Id) : IRequest<bool>, ITransactional;
