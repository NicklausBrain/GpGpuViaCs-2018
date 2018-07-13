using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessor.ImageFilters
{
    public class TplImageFilter
    {
        public static Image<Rgba32> Apply(Image<Rgba32> image, Func<Rgba32, Rgba32> filter)
        {
            Rgba32[] pixelArray = new Rgba32[image.Height * image.Width];

            image.SavePixelData(pixelArray);

            Parallel.For(0, pixelArray.Length, i => pixelArray[i] = filter(pixelArray[i]));

            return Image.LoadPixelData(
                config: Configuration.Default,
                data: pixelArray,
                width: image.Width,
                height: image.Height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
