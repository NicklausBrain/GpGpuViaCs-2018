using System;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessor.ImageFilters
{
    public class TplImageFilter
    {
        public static Image<Rgba32> Apply(Image<Rgba32> image, Func<Rgba32, Rgba32> filter)
        {
            Parallel.For(0, image.Width,
                x => Parallel.For(0, image.Height,
                    y =>
                    {
                        image[x, y] = filter(image[x, y]);
                    }));

            return image;
        }

        public static Rgba32 Invert(Rgba32 color)
        {
            return new Rgba32(
                r: (byte)(byte.MaxValue - color.R),
                g: (byte)(byte.MaxValue - color.G),
                b: (byte)(byte.MaxValue - color.B),
                a: (byte)(byte.MaxValue - color.A));
        }
    }
}
