using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Alea.Parallel;
using ImageProcessor.ImageFilters;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            string srcDir = @"..\..\..\Images\";
            string outDir = @"..\..\..\Images\Result";

            if (args.Any() && args.Length == 2)
            {
                srcDir = args[0];
                outDir = args[1];
            }

            Alea.Gpu.Default.For(0, 10000000, i => i++);


            var imagePaths = Directory.GetFiles(srcDir);

            Test("TPL", imagePaths, outDir, image => TplImageFilter.Apply(image, TplImageFilter.Invert));

            Test("AleaGPU", imagePaths, outDir, image => AleaGpuImageFilter.Apply(image, AleaGpuImageFilter.Invert));

            using (var ilGpuFilter = new IlGpuFilter())
            {
                Test("ILGPU", imagePaths, outDir, image => ilGpuFilter.Apply(image, IlGpuFilter.Invert));
            }
        }

        private static void Test(string tech, string[] imagePaths, string outDir, Func<Rgba32[], Rgba32[]> tranform)
        {
            Console.WriteLine($"Testing {tech}");

            var stopwatch = new Stopwatch();

            foreach (string imagePath in imagePaths)
            {
                Console.WriteLine($"Processing {imagePath}...");

                Image<Rgba32> image = Image.Load(imagePath);
                Rgba32[] pixelArray = new Rgba32[image.Height * image.Width];
                image.SavePixelData(pixelArray);

                string imageTitle = Path.GetFileName(imagePath);

                stopwatch.Start();
                var transformedPixels = tranform(pixelArray);
                stopwatch.Stop();

                var res =Image.LoadPixelData(
                    config: Configuration.Default,
                    data: transformedPixels,
                    width: image.Width,
                    height: image.Height);

                res.Save(Path.Combine(outDir, $"{imageTitle}.{tech}.bmp"));
            }

            Console.WriteLine($"{tech}:\t\t{stopwatch.Elapsed}");
        }
    }
}
