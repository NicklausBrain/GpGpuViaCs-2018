using System;
using System.Runtime.CompilerServices;
using Alea;
using Alea.Parallel;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessor.ImageFilters
{
    public class AleaGpuImageFilter
    {
        public static Image<Rgba32> Apply(Image<Rgba32> image, Func<Rgba32, Rgba32> filter)
        {
            Rgba32[] pixelArray = new Rgba32[image.Height * image.Width];

            image.SavePixelData(pixelArray);

            Gpu gpu = Gpu.Default;

            gpu.For(0, pixelArray.Length, x =>
            {
                pixelArray[x] = filter(pixelArray[x]);
            });

            return Image.LoadPixelData(
                config: Configuration.Default,
                data: pixelArray,
                width: image.Width,
                height: image.Height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rgba32 Invert(Rgba32 from)
        {
            var to = new Rgba32
            {
                A = (byte)~from.A,
                R = (byte)~from.R,
                G = (byte)~from.G,
                B = (byte)~from.B
            };

            return to;
        }
    }
}
