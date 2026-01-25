using L0.API.Hubs;
using L1.Core.Domain.Bidding.Events;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace L3.Worker.Consumers.Bidding;

public class AuctionEndedConsumer(IHubContext<BiddingHub> hubContext)
  : IConsumer<AuctionEndedEvent> {
  public async Task Consume(ConsumeContext<AuctionEndedEvent> context) {
    var msg = context.Message;
    await hubContext.Clients.Group(msg.AuctionId.ToString())
      .SendAsync("AuctionFinished", new {
        msg.WinnerId,
        msg.FinalPrice,
        msg.IsSold
      });
  }
}
