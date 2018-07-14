using System;
using System.Diagnostics;
using System.Linq;
using Alea.Parallel;

namespace SequenceProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            const int SequenceLength = 100000000;
            const int MaxInt = 100;
            var rn = new Random(Environment.TickCount);

            Console.WriteLine("----------------------------------");

            var intSequence = Enumerable.Range(0, SequenceLength).Select(i => (long)rn.Next(0, MaxInt)).ToArray();

            Test("PLINQ int sum", () => intSequence.AsParallel().Sum());

            Test("AleaGPU int sum", () => Alea.Gpu.Default.Sum(intSequence));

            Console.WriteLine("----------------------------------");

            intSequence = Enumerable.Range(0, SequenceLength).Select(i => (long)rn.Next(0, MaxInt)).ToArray();

            Test("PLINQ int sum", () => intSequence.AsParallel().Sum());

            Test("AleaGPU int sum", () => Alea.Gpu.Default.Sum(intSequence));

            Console.WriteLine("----------------------------------");

            var doubleSequence = Enumerable.Range(0, SequenceLength).Select(i => rn.NextDouble()).ToArray();

            Test("PLINQ double avg", () => doubleSequence.AsParallel().Average());

            Test("AleaGPU double avg", () => Alea.Gpu.Default.Average(doubleSequence));

            doubleSequence = Enumerable.Range(0, SequenceLength).Select(i => rn.NextDouble()).ToArray();

            Console.WriteLine("----------------------------------");

            Test("PLINQ double avg", () => doubleSequence.AsParallel().Average());

            Test("AleaGPU double avg", () => Alea.Gpu.Default.Average(doubleSequence));
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
