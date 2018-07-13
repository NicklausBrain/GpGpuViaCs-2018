using System;
using System.Diagnostics;

namespace MyImage
{
    public class Measure<R>
    {
        private Measure(TimeSpan time, R result)
        {
            this.Elapsed = time;
            this.Result = result;
        }

        public TimeSpan Elapsed { get; }

        public R Result { get; }

        public static Measure<R> Time(Func<R> func)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            R result = func();
            stopWatch.Stop();
            return new Measure<R>(stopWatch.Elapsed, result);
        }

        public static implicit operator R(Measure<R> measurment)
        {
            return measurment.Result;
        }
    }
}
