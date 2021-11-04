using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;

namespace Surveillance.Function.ImageProcessing.Infrastructure.Interface
{
    internal interface IEventGridService
    {
        Task PublishEventAsync(EventGridEvent @event);
    }
}
