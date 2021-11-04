using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using Surveillance.Function.ImageProcessing.Infrastructure.Interface;

namespace Surveillance.Function.ImageProcessing.Infrastructure
{
    internal class ObjectDetectionService : IObjectDetectionService
    {
        private readonly AppConfiguration appConfiguration;
        private readonly Lazy<ComputerVisionClient> visionClient;

        public ObjectDetectionService(IOptions<AppConfiguration> options)
        {
            this.appConfiguration = options?.Value ?? throw new ArgumentNullException(nameof(options));

            ServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(options.Value.CognitiveServicesApiKey);

            this.visionClient = new Lazy<ComputerVisionClient>(CreateComputerVisionClient);
        }

        public Task<DetectResult> DetectObjectsAsync(Stream image)
        {
            return this.visionClient.Value.DetectObjectsInStreamAsync(image);
        }

        private ComputerVisionClient CreateComputerVisionClient()
        {
            var credentials = new ApiKeyServiceClientCredentials(this.appConfiguration.CognitiveServicesApiKey);

            return new ComputerVisionClient(credentials)
            {
                Endpoint = this.appConfiguration.CognitiveServicesEndpoint
            };
        }
    }
}
