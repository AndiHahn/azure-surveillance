using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Surveillance.Function.ImageProcessing.Infrastructure.Interface;
using Surveillance.Function.ImageProcessing.Models;

namespace Surveillance.Function.ImageProcessing.Infrastructure
{
    internal class ImageResultRepository : IImageResultRepository
    {
        private readonly AppConfiguration appConfiguration;
        private readonly Lazy<CosmosClient> cosmosClient;

        public ImageResultRepository(IOptions<AppConfiguration> options)
        {
            this.appConfiguration = options?.Value ?? throw new ArgumentNullException(nameof(options));

            this.cosmosClient = new Lazy<CosmosClient>(CreateCosmosClient);
        }

        public Task StoreResultAsync(ImageResultEntity entity)
        {
            var container = this.cosmosClient.Value.GetContainer(
                this.appConfiguration.CosmosDbDatabaseName,
                this.appConfiguration.CosmosDbContainer);
            return container.CreateItemAsync(entity, new PartitionKey(entity.PartitionKey));
        }

        private CosmosClient CreateCosmosClient()
        {
            return new CosmosClient(this.appConfiguration.CosmosDbConnectionString);
        }
    }
}
