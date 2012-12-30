using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestTPL
{
    class Program
    {
        static void Main(string[] args)
        {
            Process current = Process.GetCurrentProcess();
            int mask=1;
            int entryCount = 200000000;
            for (int coreCount = 0; coreCount < Environment.ProcessorCount; coreCount++)
            {
                
                current.ProcessorAffinity = (IntPtr) mask;
                RunBench(entryCount, coreCount + 1);
                mask = mask * 2 + 1;
            }
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static void RunBench(int entryCount, int coreCount)
        {
            double[] entries, outputs;
            int outCount = 0;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Random rdn = new Random();
            entries = new double[entryCount];

            for (int i = 0; i < entryCount; i++)
            {
                entries[i] = rdn.NextDouble();
            }
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Parallel.ForEach<double>(entries, (val) => 
            {
//                int id=Interlocked.Increment(ref outCount) - 1;
                val = val * 512 - 256;
//                for (int x = 0; x < id; x++)
///                { }
            });
            Console.WriteLine("Processed {0} entries in {1} ms with {2} cores", entryCount, watch.ElapsedMilliseconds, coreCount);
            GC.Collect();
            watch.Stop();
            Console.WriteLine("Processed {0} entries in {1} ms with {2} cores", entryCount, watch.ElapsedMilliseconds, coreCount);
        }
    }
}
