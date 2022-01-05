using Newtonsoft.Json;

namespace Surveillance.Shared.Models
{
    public class ObjectHierarchy
    {
        public ObjectHierarchy(string objectProperty = null, double confidence = 0.0, ObjectHierarchy parent = null)
        {
            ObjectProperty = objectProperty;
            Confidence = confidence;
            Parent = parent;
        }

        [JsonConstructor]
        public ObjectHierarchy()
        {
        }

        [JsonProperty(PropertyName = "object")]
        public string ObjectProperty { get; private set; }

        [JsonProperty(PropertyName = "confidence")]
        public double Confidence { get; private set; }

        [JsonProperty(PropertyName = "parent")]
        public ObjectHierarchy Parent { get; private set; }
    }
}
