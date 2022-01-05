using Newtonsoft.Json;

namespace Surveillance.Shared.Models
{
    public class ImageMetadata
    {
        public ImageMetadata(int width, int height, string format)
        {
            Width = width;
            Height = height;
            Format = format;
        }

        [JsonConstructor]
        private ImageMetadata() { }

        [JsonProperty]
        public int Width { get; private set; }

        [JsonProperty]
        public int Height { get; private set; }

        [JsonProperty]
        public string Format { get; private set; }
    }
}
