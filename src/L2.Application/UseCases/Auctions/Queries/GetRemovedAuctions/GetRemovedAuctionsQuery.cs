using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auctions.Queries.GetRemovedAuctions;

public record GetRemovedAuctionsQuery(AuctionFilter Filter) : IRequest<GetRemovedAuctionsResult>;

public record GetRemovedAuctionsResult(List<AuctionDto> Auctions, Meta Meta);
