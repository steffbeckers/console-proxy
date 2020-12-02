using Microsoft.AspNetCore.SignalR;

namespace ConsoleProxy.API.SignalR
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("sub")?.Value;
        }
    }
}
