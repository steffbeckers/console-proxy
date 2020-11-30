using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleProxy.API.Hubs
{
    // TODO: Move
    public static class State
    {
        public static List<string> ConnectedClients { get; set; } = new List<string>();
    }

    public class RealtimeHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            State.ConnectedClients.Add(Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            State.ConnectedClients.Remove(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
