using L0.API.Hubs;
using L1.Core.Domain.Bidding.Events;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace L3.Worker.Consumers.Bidding;

public class BidPlacedConsumer(IHubContext<BiddingHub> hubContext)
  : IConsumer<BidPlacedEvent> {
  public async Task Consume(ConsumeContext<BidPlacedEvent> context) {
    var msg = context.Message;
    await hubContext.Clients.Group(msg.AuctionId.ToString())
      .SendAsync("NewBidReceived", new {
        msg.BidderId,
        msg.Amount,
        Timestamp = msg.OccurredOn
      });
  }
}
