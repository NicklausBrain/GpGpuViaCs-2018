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

        public Rgba32[] Apply(Rgba32[] pixelArray, Func<Rgba32, Rgba32> filter)
        {
            using (MemoryBuffer<Rgba32> buffer = this.gpu.Allocate<Rgba32>(pixelArray.Length))
            {
                buffer.CopyFrom(pixelArray, 0, Index.Zero, pixelArray.Length);

                this.kernel(buffer.Length, buffer.View);

                // Wait for the kernel to finish...
                this.gpu.Synchronize();

                return buffer.GetAsArray();
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
            this.gpu?.Dispose();
        }
    }
}
