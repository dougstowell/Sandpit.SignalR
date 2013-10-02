using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;


namespace Sandpit.SignalR.Web
{
    [HubName("chat")]
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }
    }
}

