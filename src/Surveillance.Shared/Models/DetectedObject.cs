using Newtonsoft.Json;

#nullable enable

namespace Surveillance.Shared.Models
{
    public class DetectedObject
    {
        public DetectedObject(BoundingRect rectangle, string objectProperty, double confidence, ObjectHierarchy? parent)
        {
            Rectangle = rectangle;
            ObjectProperty = objectProperty;
            Confidence = confidence;
            Parent = parent;
        }

        [JsonConstructor]
        private DetectedObject() { }

        [JsonProperty(PropertyName = "rectangle")]
        public BoundingRect Rectangle { get; private set; }

        [JsonProperty(PropertyName = "object")]
        public string ObjectProperty { get; private set; }

        [JsonProperty(PropertyName = "confidence")]
        public double Confidence { get; private set; }

        [JsonProperty(PropertyName = "parent")]
        public ObjectHierarchy? Parent { get; private set; }
    }
}
