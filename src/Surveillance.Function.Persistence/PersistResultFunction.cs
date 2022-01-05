using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Surveillance.Function.Persistence.Infrastructure;
using Surveillance.Function.Persistence.Models;
using Surveillance.Shared.Queue;
using System.Threading.Tasks;

namespace Surveillance.Function.Persistence
{
    public class PersistResultFunction
    {
        private readonly IImageResultRepository imageResultRepository;

        public PersistResultFunction(IImageResultRepository imageResultRepository)
        {
            this.imageResultRepository = imageResultRepository ?? throw new System.ArgumentNullException(nameof(imageResultRepository));
        }

        [FunctionName("PersistImageResultFunction")]
        public async Task PersistImageResult(
            [QueueTrigger("processed-image-queue", Connection = "ProcessedImageQueueStorageConnectionString")] ProcessedImageMessage processedImageQueueItem,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {processedImageQueueItem}");

            var entity = new ImageResultEntity(
                processedImageQueueItem.ImageUrl,
                processedImageQueueItem.DetectedPersons,
                processedImageQueueItem.Timestamp,
                processedImageQueueItem.DetectResult);

            await imageResultRepository.StoreResultAsync(entity);

            log.LogInformation("Successfully persisted image result.");
        }
    }
}
