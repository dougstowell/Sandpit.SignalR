using System;
using System.Threading;
using Microsoft.AspNet.SignalR;


namespace Sandpit.SignalR.Web
{
    public class ShapeBroadcaster
    {
        private readonly static Lazy<ShapeBroadcaster> _instance =
            new Lazy<ShapeBroadcaster>(() => new ShapeBroadcaster());

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
                    Broadcast,
                    null,
                    BroadcastInterval,
                    BroadcastInterval
                    );
        }

        public void Broadcast(object state)
        {
            // No need to send anything if our model hasn't changed
            if (_modelUpdated)
            {
                // This is how we can access the Clients property 
                // in a static hub method or outside of the hub entirely
                _hubContext.Clients.AllExcept(_model.LastUpdatedBy).updateShape(_model);
                _modelUpdated = false;
            }
        }

        public void UpdateShape(ShapeModel clientModel)
        {
            _model = clientModel;
            _modelUpdated = true;
        }
    }
}