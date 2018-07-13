using System;

namespace ImageProcessor.ImageFilters
{
    public class AleaGpuImageFilter
    {
        public static Image<Rgba32> Apply (Image<Rgba32> image, Func<Rgba32, Rgba32> filter)
        {
            Rgba32[] pixelArray = new Rgba32[image.Height * image.Width];
            image.SavePixelData(pixelArray);

            var gpu = Alea.Gpu.Default;

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

        //public delegate (byte, byte, byte) Filter(
        //    Func<byte, byte> changeA,
        //    Func<byte, byte> changeR,
        //    Func<byte, byte> changeG,
        //    Func<byte, byte> changeB);

        public static Rgba32 Invert(Rgba32 from)
        {
            var to = new Rgba32
            {
                A = (byte)(byte.MaxValue - from.A),
                R = (byte)(byte.MaxValue - from.R),
                G = (byte)(byte.MaxValue - from.G),
                B = (byte)(byte.MaxValue - from.B)
            };

            return to;
        }
    }
}
