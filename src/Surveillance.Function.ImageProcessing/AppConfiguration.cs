namespace Surveillance.Function.ImageProcessing
{
    internal class AppConfiguration
    {
        public string CognitiveServicesEndpoint { get; set; }
        public string CognitiveServicesApiKey { get; set; }
        public string EventGridTopicEndpoint { get; set; }
        public string EventGridTopicKey { get; set; }
    }
}
