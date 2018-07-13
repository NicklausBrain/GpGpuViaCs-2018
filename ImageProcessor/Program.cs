using System;
using System.IO;
using System.Linq;
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

        static void Main(string[] args)
        {
            string srcDir = @"..\..\..\Images\";
            string outDir = @"..\..\..\Images\";

            if (args.Any() && args.Length == 2)
            {
                srcDir = args[0];
                outDir = args[1];
            }

            var imagePaths = Directory.GetFiles(srcDir);

            foreach (string imagePath in imagePaths)
            {
                Image<Rgba32> image = Image.Load(imagePath);
                string imageTitle = Path.GetFileName(imagePath);

                Console.WriteLine($"Processing {imagePath}...");

                var res = MeasureTime(() =>
                {
                    var result = image.Clone();
                    result.Mutate(context => context.Invert());
                    return result;
                });

                Console.WriteLine($"ImageSharp:\t{res.Elapsed}");
                res.Result.Save(Path.Combine(outDir, $"{imageTitle}.ImageSharp.bmp"));

                res = MeasureTime(() => TplImageFilter.Apply(image, TplImageFilter.Invert));
                Console.WriteLine($"TPL:\t\t{res.Elapsed}");
                res.Result.Save(Path.Combine(outDir, $"{imageTitle}.TPL.bmp"));

                res = MeasureTime(() => AleaGpuImageFilter.Apply(image, AleaGpuImageFilter.Invert));
                Console.WriteLine($"Alea GPU:\t{res.Elapsed}");
                res.Result.Save(Path.Combine(outDir, $"{imageTitle}.AleaGPU.bmp"));

                using (var ilGpuFilter = new IlGpuFilter())
                {
                    res = MeasureTime(() => ilGpuFilter.Apply(image, IlGpuFilter.Invert));
                    Console.WriteLine($"IL GPU:\t\t{res.Elapsed}");
                    res.Result.Save(Path.Combine(outDir, $"{imageTitle}.ILGPU.bmp"));
                }
            }
        }
    }
}
