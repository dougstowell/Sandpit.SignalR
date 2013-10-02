using Newtonsoft.Json;

namespace Sandpit.SignalR.Web
{
    public class ShapeModel
    {
        [JsonProperty("left")]
        public double Left { get; set; }

        [JsonProperty("top")]
        public double Top { get; set; }

        [JsonIgnore]
        public string LastUpdatedBy { get; set; }
    }
}