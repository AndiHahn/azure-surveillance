using System;
using Newtonsoft.Json;

namespace Surveillance.Function.ImageProcessing.Models
{
    public abstract class BaseDocumentEntity
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; private set; }

        [JsonProperty(PropertyName = "PartitionKey")]
        public string PartitionKey { get; private set; }

        [JsonConstructor]
        protected BaseDocumentEntity() { }

        protected BaseDocumentEntity(string partitionKey)
        {
            this.Id = Guid.NewGuid();
            this.PartitionKey = partitionKey;
        }
    }
}
