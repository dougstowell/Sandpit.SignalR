using Microsoft.AspNet.SignalR;


namespace Sandpit.SignalR.Web
{
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }
    }
}

