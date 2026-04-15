using L0.API.Response;
using L2.Application.UseCases.Auctions.Queries.GetAuction;
using L2.Application.UseCases.Auctions.Queries.GetWonAuctions;
using L2.Application.UseCases.Bids.Commands.PlaceBid;
using L2.Application.UseCases.Bids.Queries.GetBiddingActivity;
using L2.Application.UseCases.Bids.Queries.GetBidHistory;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Bidder;

public class AuctionController : UserController {
  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetById(Guid id, CancellationToken ct) {
    var result = await Mediator.Send(new GetAuctionQuery(id), ct);
    return AppResponse.Success(result.Auction);
  }

  [HttpPost("{id:guid}/bid")]
  public async Task<IActionResult> PlaceBid(Guid id, [FromBody] PlaceBidCommand command, CancellationToken ct) {
    command = command with { AuctionId = id };
    var bidId = await Mediator.Send(command, ct);
    return AppResponse.Success(bidId, "Đặt giá thành công");
  }

  [HttpGet("{id:guid}/history")]
  public async Task<IActionResult> GetHistory(Guid id, [FromQuery] int page, [FromQuery] int pageSize,
    CancellationToken ct) {
    var result = await Mediator.Send(new GetBidHistoryQuery(id, page, pageSize), ct);
    return AppResponse.Success(result.Bids, result.Meta);
  }

  [HttpGet("my-activities")]
  public async Task<IActionResult> GetMyActivities([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetBiddingActivityQuery(sieveModel), ct);
    return AppResponse.Success(result.Auctions, result.Meta);
  }

  [HttpGet("my-wins")]
  public async Task<IActionResult> GetMyWins([FromQuery] SieveModel sieveModel, CancellationToken ct) {
    var result = await Mediator.Send(new GetWonAuctionsQuery(sieveModel), ct);
    return AppResponse.Success(result.Auctions, result.Meta);
  }
}
