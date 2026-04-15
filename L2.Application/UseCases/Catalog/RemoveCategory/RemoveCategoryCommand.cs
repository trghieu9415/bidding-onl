using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Catalog.RemoveCategory;

public record RemoveCategoryCommand(Guid Id) : IRequest<Unit>, ITransactional;
