using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Surveillance.Function.ImageProcessing;
using Surveillance.Function.ImageProcessing.Infrastructure;
using Surveillance.Function.ImageProcessing.Infrastructure.Interface;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Surveillance.Function.ImageProcessing
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

            services.AddSingleton<IObjectDetectionService, ObjectDetectionService>();
            services.AddSingleton<IEventGridService, EventGridService>();
            services.AddSingleton<IImageResultRepository, ImageResultRepository>();

            services.Configure<AppConfiguration>(configuration);

            services.AddLogging();

            services.BuildServiceProvider(true);
        }
    }
}
