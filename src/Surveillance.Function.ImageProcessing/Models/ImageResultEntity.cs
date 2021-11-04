using System;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;

namespace Surveillance.Function.ImageProcessing.Models
{
    public class ImageResultEntity : BaseDocumentEntity
    {
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
            int nrOfPersonsDetected,
            DateTime capturedAt,
            DetectResult detectResult)
            : base(Identifier.PartitionKey())
        {
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
