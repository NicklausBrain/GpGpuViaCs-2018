using System;
using System.Linq;
using ILGPU;
using ILGPU.Runtime;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessor.ImageFilters
{
    public class IlGpuFilter
    {
        public static readonly Accelerator GPU = Accelerator.Create(new Context(), Accelerator.Accelerators.First(a => a.AcceleratorType == AcceleratorType.Cuda));
        private static readonly Action<Index, ArrayView<Rgba32>> kernel =
            GPU.LoadAutoGroupedStreamKernel<Index, ArrayView<Rgba32>>(ApplyKernel);


        public static void ApplyKernel(
            Index index, // The global thread index (1D in this case)
            ArrayView<Rgba32> pixelArray// A view to a chunk of memory (1D in this case)
            )
        {
            pixelArray[index] =  Invert(pixelArray[index]);//Rgba32.RebeccaPurple;//
            //pixelArray[index] = pixelArray[index];
        }

        public static Image<Rgba32> Apply(Image<Rgba32> image, Func<Rgba32, Rgba32> filter)
        {
            {
                {
                    Rgba32[] pixelArray = new Rgba32[image.Height * image.Width];

                    using (MemoryBuffer<Rgba32> buffer = GPU.Allocate<Rgba32>(pixelArray.Length))
                    {
                        image.SavePixelData(pixelArray);

                        buffer.CopyFrom(pixelArray, 0, Index.Zero, pixelArray.Length);

                        kernel(buffer.Length, buffer.View);

                        // Wait for the kernel to finish...
                        GPU.Synchronize();

                        return Image.LoadPixelData(
                            config: Configuration.Default,
                            data: buffer.GetAsArray(),
                            width: image.Width,
                            height: image.Height);
                    }
                }
            }
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
