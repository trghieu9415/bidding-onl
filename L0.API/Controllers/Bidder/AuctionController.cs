using L0.API.Response;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.UseCases.Auctions.Commands.SearchItem;
using L2.Application.UseCases.Auctions.Queries.GetAuction;
using L2.Application.UseCases.Auctions.Queries.GetWonAuctions;
using L2.Application.UseCases.Bids.Commands.PlaceBid;
using L2.Application.UseCases.Bids.Queries.GetBiddingActivity;
using L2.Application.UseCases.Bids.Queries.GetBidHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace L0.API.Controllers.Bidder;

public class AuctionController : UserController {
  [HttpGet("{id:guid}")]
  [ProducesSuccess<AuctionDto>]
  [AllowAnonymous]
  public async Task<IActionResult> GetById(Guid id, CancellationToken ct) {
    var result = await Mediator.Send(new GetAuctionQuery(id), ct);
    return ApiResponse.Success(result.Auction);
  }

  [HttpPost("{id:guid}/bid")]
  [ProducesSuccess<IdData>]
  [EnableRateLimiting("BiddingWarPolicy")]
  public async Task<IActionResult> PlaceBid(
    Guid id, [FromBody] PlaceBidRequest req, CancellationToken ct
  ) {
    var command = new PlaceBidCommand(id, CurrentUser.Id, CurrentUser.FullName, req);
    var bidId = await Mediator.Send(command, ct);
    return ApiResponse.Success(bidId, "Đặt giá thành công");
  }

  [HttpGet("search")]
  [ProducesSuccess<List<AuctionSearchDto>>]
  [AllowAnonymous]
  public async Task<IActionResult> Search([FromQuery] AuctionSearchFilter searchFilter, CancellationToken ct) {
    var query = new SearchItemQuery(searchFilter);
    var result = await Mediator.Send(query, ct);
    return ApiResponse.Success(result.Items, result.Meta);
  }

  [HttpGet("{id:guid}/history")]
  [ProducesSuccess<List<BidDto>>]
  public async Task<IActionResult> GetHistory(Guid id, [FromQuery] int page, [FromQuery] int perPage,
    CancellationToken ct) {
    var result = await Mediator.Send(new GetBidHistoryQuery(id, page, perPage), ct);
    return ApiResponse.Success(result.Bids, result.Meta);
  }

  [HttpGet("my-activities")]
  [ProducesSuccess<List<AuctionDto>>]
  public async Task<IActionResult> GetMyActivities(
    [FromQuery] AuctionFilter filter,
    CancellationToken ct
  ) {
    var result = await Mediator.Send(new GetBiddingActivityQuery(CurrentUser.Id, filter), ct);
    return ApiResponse.Success(result.Auctions, result.Meta);
  }

  [HttpGet("my-wins")]
  [ProducesSuccess<List<AuctionDto>>]
  public async Task<IActionResult> GetMyWins([FromQuery] AuctionFilter filter, CancellationToken ct) {
    var result = await Mediator.Send(new GetWonAuctionsQuery(CurrentUser.Id, filter), ct);
    return ApiResponse.Success(result.Auctions, result.Meta);
  }
}
