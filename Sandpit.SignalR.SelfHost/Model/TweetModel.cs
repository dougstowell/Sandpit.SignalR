using System;

namespace Sandpit.SignalR.SelfHost
{
    public class TweetModel
    {
        public String Id { get; set; }
        public String UserName { get; set; }
        public String Text { get; set; }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
        public String Source { get; set; }
        public DateTime CreatedAt { get; set; }

        public String ImageSource { get; set; }
    }
}