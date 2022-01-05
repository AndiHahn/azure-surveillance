using Surveillance.Shared.Models;
using System;

namespace Surveillance.Shared.Queue
{
    public class ProcessedImageMessage
    {
        public string ImageUrl { get; set; }

        public int DetectedPersons { get; set; }

        public DateTime Timestamp { get; set; }

        public DetectResult DetectResult { get; set; }

        public override string ToString()
        {
            return $"{nameof(ImageUrl)}: {ImageUrl}, {nameof(DetectedPersons)}: {DetectedPersons}, {nameof(Timestamp)}: {Timestamp}, {nameof(DetectResult)}: {DetectResult}";
        }
    }
}
