using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Surveillance.Function.ImageProcessing.Infrastructure.Interface
{
    internal interface IObjectDetectionService
    {
        Task<DetectResult> DetectObjectsAsync(Stream image);
    }
}
