using System.Threading.Tasks;
using Surveillance.Function.ImageProcessing.Models;

namespace Surveillance.Function.ImageProcessing.Infrastructure.Interface
{
    internal interface IImageResultRepository
    {
        Task StoreResultAsync(ImageResultEntity entity);
    }
}
