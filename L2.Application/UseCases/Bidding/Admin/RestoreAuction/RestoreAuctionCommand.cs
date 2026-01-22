using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.RestoreAuction;

public record RestoreAuctionCommand(Guid Id) : ICommand<Unit>;