using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

#nullable enable

namespace Surveillance.Function.ImageProcessing.Models
{
    internal static class Mapper
    {
        public static Shared.Models.BoundingRect ToModel(this BoundingRect rect)
            => new Shared.Models.BoundingRect(rect.X, rect.Y, rect.W, rect.H);

        public static Shared.Models.ImageMetadata ToModel(this ImageMetadata metaData)
            => new Shared.Models.ImageMetadata(metaData.Width, metaData.Height, metaData.Format);

        public static Shared.Models.ObjectHierarchy? ToModel(this ObjectHierarchy hierarchy)
            => hierarchy is null ? null : new Shared.Models.ObjectHierarchy(hierarchy.ObjectProperty, hierarchy.Confidence, hierarchy.ToModel());

        public static Shared.Models.DetectedObject ToModel(this DetectedObject @object)
            => new Shared.Models.DetectedObject(@object.Rectangle.ToModel(), @object.ObjectProperty, @object.Confidence, @object.Parent.ToModel());
    }
}
