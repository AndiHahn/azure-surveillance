// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Surveillance.Function.ImageProcessing.Infrastructure.Interface;
using Surveillance.Function.ImageProcessing.Models;
using Surveillance.Shared.Models;
using Surveillance.Shared.Queue;

namespace Surveillance.Function.ImageProcessing
{
    internal class ImageUploadedFunction
    {
        public const double ThresholdPersonDetected = 0.7;
        public const string ObjectPersonIdentifier = "person";

        private readonly IObjectDetectionService objectDetectionService;

        public ImageUploadedFunction(
            IObjectDetectionService objectDetectionService)
        {
            this.objectDetectionService = objectDetectionService ?? throw new System.ArgumentNullException(nameof(objectDetectionService));
        }

        [FunctionName("ImageUploadedFunction")]
        public async Task ImageUploadedRun(
            [QueueTrigger("image-uploaded-queue", Connection = "ImageUploadedQueueStorageConnectionString")] EventGridEvent blobCreatedQueueItem,
            [Queue("processed-image-queue", Connection = "ProcessedImageQueueStorageConnectionString")] IAsyncCollector<string> processedImageQueue,
            [Queue("person-detected-queue", Connection = "PersonDetectedQueueStorageConnectionString")] IAsyncCollector<string> personDetectedQueue,
            [Blob("{data.url}", FileAccess.Read, Connection = "ImageStorageConnectionString")] Stream inputImage,
            ILogger log)
        {
            log.LogInformation(blobCreatedQueueItem.Data.ToString());

            var detectionResult = await objectDetectionService.DetectObjectsAsync(inputImage);

            var detectedPersons = detectionResult.Objects
                .Where(o => o.ObjectProperty.ToLower().Contains(ObjectPersonIdentifier));
            if (detectedPersons.Any())
            {
                log.LogInformation("Add message to person detected queue...");

                double maxConfidence = detectedPersons.Max(p => p.Confidence);

                PersonDetectedMessage personDetectedMessage = new PersonDetectedMessage
                {
                    Confidence = maxConfidence,
                    Timestamp = blobCreatedQueueItem.EventTime
                };

                await personDetectedQueue.AddAsync(JsonConvert.SerializeObject(personDetectedMessage));
            }
            else
            {
                log.LogInformation("No person detected.");
            }

            log.LogInformation("Add message to processed image queue...");

            var deserializedEventData = JsonConvert.DeserializeObject<BlobCreatedEventData>(blobCreatedQueueItem.Data.ToString());

            ProcessedImageMessage message = new ProcessedImageMessage
            {
                ImageUrl = deserializedEventData.Url,
                DetectedPersons = detectedPersons.Count(),
                Timestamp = blobCreatedQueueItem.EventTime,
                DetectResult = new DetectResult
                {
                    RequestId = detectionResult.RequestId,
                    ModelVersion = detectionResult.ModelVersion,
                    Metadata = detectionResult.Metadata.ToModel(),
                    Objects = detectionResult.Objects.Select(o => o.ToModel()).ToList()
                }
            };

            await processedImageQueue.AddAsync(JsonConvert.SerializeObject(message));
                        
            log.LogInformation("Finished image uploaded processing.");
        }
    }
}
