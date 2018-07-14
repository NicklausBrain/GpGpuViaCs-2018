using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Alea.Parallel;

namespace SequenceProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            // warming up GPU
            Alea.Gpu.Default.For(0, 10000000, i => i++);

            var intSequence = Enumerable.Range(0, 10000).ToArray();

            Test("TPL int sum", () => intSequence.AsParallel().Sum());

            Test("AleaGPU int sum", () => Alea.Gpu.Default.Sum(intSequence));
            intSequence = intSequence.Reverse().ToArray();
            Test("AleaGPU int sum", () => Alea.Gpu.Default.Sum(intSequence));

            var bigIntSequence = Enumerable.Range(0, 1000000).Select(i => new BigInteger(i)).ToArray();

            Test("TPL BigInteger sum", () => bigIntSequence.AsParallel().Aggregate((a, b) => a + b));

            //Alea.Gpu.Default.Aggregate(bigIntSequence, (a, b) => a + b);
            //Test("AleaGPU BigInteger sum", () => Alea.Gpu.Default.Aggregate(bigIntSequence, (a, b) => a + b));
        }



        private static void Test<T>(string tech, Func<T> reduce) where T : struct
        {
            Console.WriteLine($"Testing {tech}");

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var res = reduce();
            stopwatch.Stop();

            Console.WriteLine($"{tech} : {res} :{stopwatch.Elapsed}");
        }
    }
}
