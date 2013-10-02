using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;


namespace Sandpit.SignalR.Web
{
    [HubName("moveShape")]
    public class MoveShapeHub : Hub
    {
        private ShapeBroadcaster _broadcaster;

        public MoveShapeHub()
            : this(ShapeBroadcaster.Instance)
        {
        }

        public MoveShapeHub(ShapeBroadcaster broadcaster)
        {
            _broadcaster = broadcaster;
        }

        public void UpdateModel(ShapeModel clientModel)
        {
            clientModel.LastUpdatedBy = Context.ConnectionId;
            _broadcaster.UpdateShape(clientModel);
        }

        public override Task OnConnected()
        {
            _broadcaster.AddConnection(Context.ConnectionId);
            return null;
        }

        public override Task OnDisconnected()
        {
            _broadcaster.RemoveConnection(Context.ConnectionId);
            return null;
        }

        public override Task OnReconnected()
        {
            _broadcaster.AddConnection(Context.ConnectionId);
            return null;
        }
    }
}