using Newtonsoft.Json;
using System.Collections.Generic;

namespace Surveillance.Shared.Models
{
    public class DetectResult
    {
        [JsonProperty(PropertyName = "requestId")]
        public string RequestId { get; set; }

        [JsonProperty(PropertyName = "modelVersion")]
        public string ModelVersion { get; set; }

        [JsonProperty(PropertyName = "metadata")]
        public ImageMetadata Metadata { get; set; }

        [JsonProperty(PropertyName = "objects")]
        public IList<DetectedObject> Objects { get; set; }
    }
}
