using L0.API.Response;
using L2.Application.UseCases.Bidding.Bidder.GetAuction;
using L2.Application.UseCases.Bidding.Bidder.GetBiddingActivity;
using L2.Application.UseCases.Bidding.Bidder.GetBidHistory;
using L2.Application.UseCases.Bidding.Bidder.GetWonAuctions;
using L2.Application.UseCases.Bidding.Bidder.PlaceBid;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace L0.API.Controllers.Bidder;

public class AuctionController(IMediator mediator) : UserController {
  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetById(Guid id) {
    var result = await mediator.Send(new GetAuctionQuery(id));
    return AppResponse.Success(result.Auction);
  }

  [HttpPost("{id:guid}/bid")]
  public async Task<IActionResult> PlaceBid(Guid id, [FromBody] PlaceBidCommand command) {
    command = command with { AuctionId = id };
    var bidId = await mediator.Send(command);
    return AppResponse.Success(bidId, "Đặt giá thành công");
  }

  [HttpGet("{id:guid}/history")]
  public async Task<IActionResult> GetHistory(Guid id, [FromQuery] SieveModel sieveModel) {
    var result = await mediator.Send(new GetBidHistoryQuery(id, sieveModel));
    return AppResponse.Success(result.Bids, result.Meta);
  }

  [HttpGet("my-activities")]
  public async Task<IActionResult> GetMyActivities([FromQuery] SieveModel sieveModel) {
    var result = await mediator.Send(new GetBiddingActivityQuery(sieveModel));
    return AppResponse.Success(result.Auctions, result.Meta);
  }

  [HttpGet("my-wins")]
  public async Task<IActionResult> GetMyWins([FromQuery] SieveModel sieveModel) {
    var result = await mediator.Send(new GetWonAuctionsQuery(sieveModel));
    return AppResponse.Success(result.Auctions, result.Meta);
  }
}
