using System;
using Newtonsoft.Json;
using Surveillance.Shared.Models;

namespace Surveillance.Function.Persistence.Models
{
    public class ImageResultEntity : BaseDocumentEntity
    {
        [JsonProperty]
        public string ImageUrl { get; private set; }

        [JsonProperty]
        public int NrOfPersonsDetected { get; private set; }

        [JsonProperty]
        public DateTime CapturedAt { get; private set; }

        [JsonProperty]
        public DetectResult DetectResult { get; private set; }

        [JsonConstructor]
        private ImageResultEntity()
        {
        }

        public ImageResultEntity(
            string imageUrl,
            int nrOfPersonsDetected,
            DateTime capturedAt,
            DetectResult detectResult)
            : base(Identifier.PartitionKey())
        {
            this.ImageUrl = imageUrl;
            this.NrOfPersonsDetected = nrOfPersonsDetected;
            this.CapturedAt = capturedAt;
            this.DetectResult = detectResult;
        }

        public static class Identifier
        {
            public static string PartitionKey() => "ImageResult";
        }
    }
}
