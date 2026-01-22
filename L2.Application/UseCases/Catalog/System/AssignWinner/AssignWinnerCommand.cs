using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Catalog.System.AssignWinner;

public record AssignWinnerCommand(Guid CatalogItemId, bool IsSold) : ICommand<Unit>;