using System;
using System.Threading.Tasks;

namespace ProofOfConcept.ThreadAndTask
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            TestThread();

            Console.WriteLine("\n\rMain said: Press key to exits!");
            Console.ReadLine();
        }

        static void TestThread()
        {
            Console.WriteLine("\r\n========================= THREAD =========================");
            ThreadExample.RunNewThread();
            ThreadExample.RunNewThreadWithParameter();
            ThreadPoolExample.QueueThread();
        }

        static async Task TestTaskAsync()
        {
            Console.WriteLine("\r\n========================= TASK =========================");
            TaskExample.CreateAndRunTask();
            // Compare time: CreateAndRunTaskWithWait and CreateAndRunTaskWithWhenAll
            await TaskExample.CreateAndRunTaskWithWait();
            await TaskExample.CreateAndRunTaskWithWhenAll();

            await TaskExample.CreateAndRunTaskWithResult();
        }

        static void TestParallel()
        {
            Console.WriteLine("\r\n========================= PARALLEL =========================");
            int length = 50000000; // 500000 vs 50000000
            ParallelExample.NotUse(length);
            ParallelExample.UseWithLinq(length);
            ParallelExample.UseWithTask();
            Console.WriteLine();
            Console.WriteLine();
            ParallelExample.UseWithMutilTaskNotUseStageObject();
            Console.WriteLine();
            ParallelExample.UseWithMutilTaskUseStageObject();
        }

        static void TestBarrier()
        {
            Console.WriteLine("\r\n========================= BARRIER =========================");
            BarrierExample.BarrierSample();
        }

        static void TestAutoResetEvent()
        {
            Console.WriteLine("\r\n========================= AUTO RESET EVENT EXAMPLE =========================");
            AutoResetEventExample.Sample();
        }

        static void TestCancellationToken()
        {
            Console.WriteLine("\r\n========================= CANCELLATION TOKEN =========================");
            CancellationTokenExample.Sample();
        }
    }
}
