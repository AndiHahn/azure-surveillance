using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Surveillance.Function.ImageProcessing.Infrastructure
{
    internal interface IObjectDetectionService
    {
        Task<DetectResult> DetectObjectsAsync(Stream image);
    }
}
