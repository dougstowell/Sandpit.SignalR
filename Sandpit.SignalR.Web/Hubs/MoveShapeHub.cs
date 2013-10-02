using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;


namespace Sandpit.SignalR.Web
{
    [HubName("moveShape")]
    public class MoveShapeHub : Hub
    {
        // Is set via the constructor on each creation
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
            // Update the shape model within our broadcaster
            _broadcaster.UpdateShape(clientModel);
        }
    }
}