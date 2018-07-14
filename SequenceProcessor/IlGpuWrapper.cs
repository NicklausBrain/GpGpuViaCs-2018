using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILGPU;
using ILGPU.Lightning;
using ILGPU.ReductionOperations;
using ILGPU.Runtime;
using ILGPU.ShuffleOperations;

namespace SequenceProcessor
{
    public class IlGpuWrapper : IDisposable
    {
        private readonly Accelerator gpu;
        //private readonly Action<Index, ArrayView<int>> intSumKernel;

        public IlGpuWrapper()
        {
            this.gpu = Accelerator.Create(new Context(), Accelerator.Accelerators.First(a => a.AcceleratorType == AcceleratorType.Cuda));
            //this.intSumKernel = this.gpu.LoadAutoGroupedStreamKernel<Index, ArrayView<int>>(ApplyKernel);
        }

        public int Sum(int[] arr)
        {
            using (var buffer = this.gpu.Allocate<int>(arr.Length))
            {
                buffer.CopyFrom(arr, 0, Index.Zero, arr.Length);

                using (var target = this.gpu.Allocate<int>(1))
                {
                    this.gpu.Reduce(
                        buffer.View,
                        target.View,
                        new ShuffleDownInt32(),
                        new AddInt32());

                    this.gpu.Synchronize();

                    return target.GetAsArray()[0];
                }
            }
        }

        //public static void ApplyKernel(
        //    Index index, /* The global thread index (1D in this case) */
        //    ArrayView<Rgba32> pixelArray /* A view to a chunk of memory (1D in this case)*/)
        //{
        //    pixelArray[index] = Invert(pixelArray[index]);
        //}

        public void Dispose()
        {
            this.gpu?.Dispose();
        }
    }
}
