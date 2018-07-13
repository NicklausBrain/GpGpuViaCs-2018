using System;
using System.Linq;
using System.Runtime.CompilerServices;
using ILGPU;
using ILGPU.Runtime;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessor.ImageFilters
{
    public class IlGpuFilter : IDisposable
    {
        private readonly Accelerator gpu;
        private readonly Action<Index, ArrayView<Rgba32>> kernel;

        public IlGpuFilter()
        {
            this.gpu = Accelerator.Create(new Context(), Accelerator.Accelerators.First(a => a.AcceleratorType == AcceleratorType.Cuda));
            this.kernel = this.gpu.LoadAutoGroupedStreamKernel<Index, ArrayView<Rgba32>>(ApplyKernel);
        }

        public static void ApplyKernel(
            Index index, /* The global thread index (1D in this case) */
            ArrayView<Rgba32> pixelArray /* A view to a chunk of memory (1D in this case)*/)
        {
            pixelArray[index] = Invert(pixelArray[index]);
        }

        public Image<Rgba32> Apply(Image<Rgba32> image, Func<Rgba32, Rgba32> filter)
        {
            Rgba32[] pixelArray = new Rgba32[image.Height * image.Width];

            using (MemoryBuffer<Rgba32> buffer = this.gpu.Allocate<Rgba32>(pixelArray.Length))
            {
                image.SavePixelData(pixelArray);

                buffer.CopyFrom(pixelArray, 0, Index.Zero, pixelArray.Length);

                this.kernel(buffer.Length, buffer.View);

                // Wait for the kernel to finish...
                this.gpu.Synchronize();

                return Image.LoadPixelData(
                    config: Configuration.Default,
                    data: buffer.GetAsArray(),
                    width: image.Width,
                    height: image.Height);
            }
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

        public void Dispose()
        {
            //this.context?.Dispose();
            this.gpu?.Dispose();
        }
    }
}
