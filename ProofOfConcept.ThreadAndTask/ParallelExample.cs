using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProofOfConcept.ThreadAndTask
{
    /// <summary>
    /// ParallelExample
    /// Document: https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/
    /// </summary>
    public static class ParallelExample
    {
        /// <summary>
        /// Not use Parallel
        /// </summary>
        public static void NotUse(int length)
        {

            Console.WriteLine("\r\nNotUse Parallel");
            var source = Enumerable.Range(1, length).ToList();
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
         
            var evenNums = new List<double>();

            for (int i = 0; i < source.Count; i++)
            {
                if(source[i] % 2 == 0)
                {
                    evenNums.Add(source[i]);
                }
            }

            Console.WriteLine("{0} even numbers out of {1} total",
                              evenNums.Sum(), source.Count);

            Console.WriteLine($"Execution Time Not use parallel: {watch.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// UseWithLinq
        /// </summary>
        public static void UseWithLinq(int length)
        {
            var source = Enumerable.Range(1, length).Select<int,double>(x =>x).ToList();
            Console.WriteLine("\r\n Use Parallel");

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
          

            // Opt in to PLINQ with AsParallel.
            var evenNums = from num in source.AsParallel()
                           where num % 2 == 0
                           select num;

            Console.WriteLine("{0} even numbers out of {1} total",
                              evenNums.Sum(), source.Count);

            Console.WriteLine($"Execution Time Parallel: {watch.ElapsedMilliseconds} ms");
        }


        /// <summary>
        /// Task Parallel Library UseWithTask
        /// </summary>
        public static void UseWithTask()
        {
            Thread.CurrentThread.Name = "Main";

            // Define and run the task.
            Task taskA = Task.Run(() => Console.WriteLine("Hello from taskA."));

            // Output a message from the calling thread.
            Console.WriteLine("Hello from thread '{0}'.",
                                Thread.CurrentThread.Name);
            taskA.Wait();
        }

        /// <summary>
        /// Task Parallel Library UseWithMutilTask UseWithMutilTaskNotUseStageObject => no capture outer variables
        /// </summary>
        public static void UseWithMutilTaskNotUseStageObject()
        {
            // Create the task object by using an Action(Of Object) to pass in the loop
            // counter. This produces an unexpected result.
            Task[] taskArray = new Task[10];
            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew((object obj) =>
                {
                    var data = new CustomData() { Name = i, CreationTime = DateTime.Now.Ticks };
                    data.ThreadNum = Thread.CurrentThread.ManagedThreadId;
                    Console.WriteLine("Task #{0} created at {1} on thread #{2}.",
                                      data.Name, data.CreationTime, data.ThreadNum);
                }, i);
            }
            Task.WaitAll(taskArray);
        }

        /// <summary>
        /// Task Parallel Library UseWithMutilTaskUseStageObject =>  capture outer variables
        /// </summary>
        public static void UseWithMutilTaskUseStageObject()
        {
            // Create the task object by using an Action(Of Object) to pass in custom data
            // to the Task constructor. This is useful when you need to capture outer variables
            // from within a loop.
            Task[] taskArray = new Task[10];
            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew((Object obj) =>
                {
                    CustomData data = obj as CustomData;
                    if (data == null)
                        return;

                    data.ThreadNum = Thread.CurrentThread.ManagedThreadId;
                    Console.WriteLine("Task #{0} created at {1} on thread #{2}.",
                                     data.Name, data.CreationTime, data.ThreadNum);
                },  new CustomData() { Name = i, CreationTime = DateTime.Now.Ticks });
            }
            Task.WaitAll(taskArray);
        }
    }

    class CustomData
    {
        public long CreationTime;
        public int Name;
        public int ThreadNum;
    }
}
