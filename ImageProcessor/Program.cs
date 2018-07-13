using System;
using System.Diagnostics;
using ImageProcessor.ImageFilters;
using MyImage;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Filters;

namespace ImageProcessor
{
    class Program
    {
        private static readonly Func<Func<Image<Rgba32>>, Measure<Image<Rgba32>>> MeasureTime = new Func<Func<Image<Rgba32>>, Measure<Image<Rgba32>>>(Measure<Image<Rgba32>>.Time);

        static void Main()
        {
            string output = @"D:\!kira\";
            Image<Rgba32> image = Image.Load(@"D:\!kira\kira.bmp");
            Rgba32[] arr = new Rgba32[image.Width * image.Height];
            image.SavePixelData(arr);

            var res = MeasureTime(() => ProcessSharp(image.Clone()));
            Console.WriteLine($"ImageSharp:\t{res.Elapsed}");
            res.Result.Save(output + "imageSharp.bmp");

            res = MeasureTime(() => TplImageFilter.Apply(image.Clone(), TplImageFilter.Invert));
            Console.WriteLine($"TPL:\t\t{res.Elapsed}");
            res.Result.Save(output + "tpl.bmp");

            res = MeasureTime(() => AleaGpuImageFilter.Apply(image, AleaGpuImageFilter.Invert));
            Console.WriteLine($"Alea GPU:\t{res.Elapsed}");
            res.Result.Save(output + "aleaGpu1.bmp");
            res = MeasureTime(() => AleaGpuImageFilter.Apply(image, AleaGpuImageFilter.Invert));
            Console.WriteLine($"Alea GPU:\t{res.Elapsed}");
            res.Result.Save(output + "aleaGpu2.bmp");

            res = MeasureTime(() => IlGpuFilter.Apply(image, IlGpuFilter.Invert));
            Console.WriteLine($"IL GPU:\t\t{res.Elapsed}");
            res.Result.Save(output + "ilgpu.bmp");

            res = MeasureTime(() => IlGpuFilter.Apply(image, IlGpuFilter.Invert));
            Console.WriteLine($"IL GPU:\t\t{res.Elapsed}");
            res.Result.Save(output + "ilgpu2.bmp");

            Process.GetCurrentProcess().Kill();
        }

        static Image<Rgba32> ProcessSharp(Image<Rgba32> image)
        {
            image.Mutate(context => context.Invert());
            return image;
        }

        static Image<Rgba32> ProcessSingleThread(Image<Rgba32> image)
        {
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Rgba32 color = image[x, y];
                    image[x, y] = new Rgba32(0, color.G, color.B, color.A);
                }
            }

            return image;
        }
    }
}
