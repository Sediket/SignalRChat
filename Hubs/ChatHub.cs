using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            ///receive board in json
            ///deserialize the board from json
            ///do board calculations
            ///serialize the board back into json
            ///send to all clients
            
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}