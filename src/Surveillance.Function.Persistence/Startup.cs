using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Surveillance.Function.Persistence;
using Surveillance.Function.Persistence.Infrastructure;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Surveillance.Function.Persistence
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            ConfigureServices(builder.Services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            services.AddSingleton<IImageResultRepository, ImageResultRepository>();

            services.Configure<AppConfiguration>(configuration);

            services.AddLogging();

            services.BuildServiceProvider(true);
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            string environment = Environment.GetEnvironmentVariable("Environment");
            string keyVaultUri = Environment.GetEnvironmentVariable("AzureKeyVaultUri");
            string appConfigSecretKey = Environment.GetEnvironmentVariable("AzureAppConfigSecretKey");

            var credentialOptions = new DefaultAzureCredentialOptions
            {
                ExcludeAzureCliCredential = true,
                ExcludeAzurePowerShellCredential = true,
                ExcludeEnvironmentCredential = true,
                ExcludeInteractiveBrowserCredential = true,
                ExcludeManagedIdentityCredential = false,
                ExcludeSharedTokenCacheCredential = true,
                ExcludeVisualStudioCodeCredential = true,
                ExcludeVisualStudioCredential = false
            };

            var credential = new DefaultAzureCredential(credentialOptions);

            var secretClient = new SecretClient(new Uri(keyVaultUri), credential);
            string appConfigConnectionString = secretClient.GetSecret(appConfigSecretKey).Value.Value;

            builder.ConfigurationBuilder.AddAzureAppConfiguration(
                options =>
                {
                    options.Connect(appConfigConnectionString)
                        .Select(KeyFilter.Any, LabelFilter.Null)
                        .Select(KeyFilter.Any, environment)
                        .ConfigureKeyVault(
                            vault =>
                            {
                                vault.SetCredential(credential);
                            });
                });
        }
    }
}