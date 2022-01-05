namespace Surveillance.Function.Persistence
{
    internal class AppConfiguration
    {
        public string CosmosDbConnectionString { get; set; }

        public string CosmosDbDatabaseName { get; set; }

        public string CosmosDbContainer { get; set; }
    }
}
