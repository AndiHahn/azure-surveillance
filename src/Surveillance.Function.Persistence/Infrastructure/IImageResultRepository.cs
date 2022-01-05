using Surveillance.Function.Persistence.Models;
using System.Threading.Tasks;

namespace Surveillance.Function.Persistence.Infrastructure
{
    public interface IImageResultRepository
    {
        Task StoreResultAsync(ImageResultEntity entity);
    }
}
