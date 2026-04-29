using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auctions.Queries.GetWonAuctions;

public record GetWonAuctionsQuery(Guid UserId, AuctionFilter Filter) : IRequest<GetWonAuctionsResult>;

public record GetWonAuctionsResult(List<AuctionDto> Auctions, Meta Meta);
