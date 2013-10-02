using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.AspNet.SignalR;


namespace Sandpit.SignalR.Web
{
    public class ShapeBroadcaster
    {
        private readonly static Lazy<ShapeBroadcaster> _instance =
            new Lazy<ShapeBroadcaster>(() => new ShapeBroadcaster());

        private readonly ConcurrentDictionary<string, Object> _connections = new ConcurrentDictionary<string, Object>();

        // We're going to broadcast to all clients a maximum of 25 times per second
        private readonly TimeSpan BroadcastInterval = TimeSpan.FromMilliseconds(40);
        private readonly IHubContext _hubContext;
        private Timer _broadcastLoop;
        private ShapeModel _model;
        private bool _modelUpdated;


        public static ShapeBroadcaster Instance
        {
            get
            {
                return _instance.Value;
            }
        }


        public ShapeBroadcaster()
        {
            // Save our hub context so we can easily use it 
            // to send to its connected clients
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<MoveShapeHub>();

            _model = new ShapeModel();
            _modelUpdated = false;

            // Start the broadcast loop
            _broadcastLoop =
                new Timer
                    (
                    BroadcastUpdateShape,
                    null,
                    BroadcastInterval,
                    BroadcastInterval
                    );
        }


        public void UpdateShape(ShapeModel clientModel)
        {
            _model = clientModel;
            _modelUpdated = true;
        }

        public void AddConnection(String connectionId)
        {
            _connections.TryAdd(connectionId, null);
            BroadcastClientCountChange();
        }

        public void RemoveConnection(String connectionId)
        {
            Object val;
            _connections.TryRemove(connectionId, out val);
            BroadcastClientCountChange();
        }

        public void BroadcastUpdateShape(object state)
        {
            if (_modelUpdated)
            {
                _hubContext.Clients.AllExcept(_model.LastUpdatedBy).updateShape(_model);
                _modelUpdated = false;
            }
        }

        public void BroadcastClientCountChange()
        {
            _hubContext.Clients.All.clientCountChanged(_connections.Count);
        }
    }
}