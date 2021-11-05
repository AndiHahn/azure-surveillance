// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Surveillance.Function.ImageProcessing.Infrastructure.Interface;
using Surveillance.Function.ImageProcessing.Models;

namespace Surveillance.Function.ImageProcessing
{
    internal class ImageUploadedFunction
    {
        public const double ThresholdPersonDetected = 0.7;
        public const string ObjectPersonIdentifier = "person";

        private readonly IObjectDetectionService objectDetectionService;
        private readonly IEventGridService eventGridService;
        private readonly IImageResultRepository imageResultRepository;

        public ImageUploadedFunction(
            IObjectDetectionService objectDetectionService,
            IEventGridService eventGridService,
            IImageResultRepository imageResultRepository)
        {
            this.objectDetectionService = objectDetectionService ?? throw new System.ArgumentNullException(nameof(objectDetectionService));
            this.eventGridService = eventGridService ?? throw new System.ArgumentNullException(nameof(eventGridService));
            this.imageResultRepository = imageResultRepository ?? throw new System.ArgumentNullException(nameof(imageResultRepository));
        }

        [FunctionName("ImageUploadedFunction")]
        public async Task ImageUploadedRun(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            [Blob("{data.url}", FileAccess.Read, Connection = "AzureBlobStorageConnectionString")] Stream inputImage,
            ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());

            var detectionResult = await objectDetectionService.DetectObjectsAsync(inputImage);

            var detectedPersons = detectionResult.Objects
                .Where(o => o.ObjectProperty.ToLower().Contains(ObjectPersonIdentifier));
            if (detectedPersons.Any())
            {
                log.LogInformation("Publish person detected event...");

                double maxConfidence = detectedPersons.Max(p => p.Confidence);
                var personDetectedEvent = new PersonDetectedEvent(maxConfidence, eventGridEvent.EventTime);
                await eventGridService.PublishEventAsync(personDetectedEvent);
            }
            else
            {
                log.LogInformation("No person detected.");
            }

            log.LogInformation("Store image detection result...");


            var deserializedEventData = JsonConvert.DeserializeObject<BlobCreatedEventData>(eventGridEvent.Data.ToString());

            var entity = new ImageResultEntity(
                deserializedEventData.Url,
                detectedPersons.Count(),
                eventGridEvent.EventTime,
                detectionResult);
            await imageResultRepository.StoreResultAsync(entity);

            log.LogInformation("Finished image uploaded processing.");
        }
    }
}
