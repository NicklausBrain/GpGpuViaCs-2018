using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessor.ImageFilters
{
    public class TplImageFilter
    {
        public static Rgba32[] Apply(Rgba32[] pixelArray, Func<Rgba32, Rgba32> filter)
        {
            Parallel.For(0, pixelArray.Length, i => pixelArray[i] = filter(pixelArray[i]));

            return pixelArray;
        }

        public static Rgba32 Invert(Rgba32 color)
        {
            return new Rgba32(
                r: (byte)~color.R,
                g: (byte)~color.G,
                b: (byte)~color.B,
                a: (byte)~color.A);
        }
    }
}
