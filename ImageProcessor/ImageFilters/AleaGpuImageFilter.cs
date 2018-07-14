using System;
using System.Runtime.CompilerServices;
using Alea;
using Alea.Parallel;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessor.ImageFilters
{
    public class AleaGpuImageFilter
    {
        public static Rgba32[] Apply(Rgba32[] pixelArray, Func<Rgba32, Rgba32> filter)
        {
            Gpu gpu = Gpu.Default;

            //var mem = gpu.ArrayGetMemory(pixelArray, Access.ReadWrite);

            gpu.For(0, pixelArray.Length, x =>
            {
                pixelArray[x] = filter(pixelArray[x]);
            });

            return pixelArray;
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
