using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.RemoveAuction;

public record RemoveAuctionCommand(Guid Id) : ICommand<Unit>;