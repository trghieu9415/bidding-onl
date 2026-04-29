using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace L0.API.Hubs;

[Authorize]
public class UserHub : Hub;
