using Newtonsoft.Json;

namespace Surveillance.Shared.Models
{
    public class BoundingRect
    {
        public BoundingRect(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        [JsonConstructor]
        private BoundingRect() { }

        [JsonProperty(PropertyName = "x")]
        public int X { get; private set; }

        [JsonProperty(PropertyName = "y")]
        public int Y { get; private set; }

        [JsonProperty(PropertyName = "w")]
        public int W { get; private set; }

        [JsonProperty(PropertyName = "h")]
        public int H { get; private set; }
    }
}
