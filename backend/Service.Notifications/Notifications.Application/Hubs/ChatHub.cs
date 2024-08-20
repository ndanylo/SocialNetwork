using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Notifications.Application.Hubs.Abstraction;

namespace Notifications.Application.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHub> { }
}
