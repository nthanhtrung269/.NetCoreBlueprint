using System;
using System.Threading;

namespace ProofOfConcept.ThreadAndTask
{
    /// <summary>
    /// ThreadPoolExample
    /// Provides a pool of threads that can be used to execute tasks,
    /// post work items, process asynchronous I/O, wait on behalf of other threads, and process timers.ss
    /// Document: https://docs.microsoft.com/en-us/dotnet/api/system.threading.threadpool?view=net-5.0
    /// Examples of operations: Task, System.Threading.Timer, ...
    /// </summary>
    public static class ThreadPoolExample
    {
        public static void QueueThread()
        {
            // Queue the task.
            ThreadPool.QueueUserWorkItem(ThreadProc1);
            ThreadPool.QueueUserWorkItem(ThreadProc2, "pass parameter to thread");
            Console.WriteLine("Main thread does some work, then sleeps.");

            Console.WriteLine("Main thread exits.");
        }

        static void ThreadProc1(object stateInfo)
        {
            // No state object was passed to ThreadProc1, so stateInfo is null.
            Console.WriteLine("Hello from the thread pool (1).");
        }

        static void ThreadProc2(object stateInfo)
        {
            Console.WriteLine("Hello from the thread pool (2): {0}.",stateInfo?.ToString());
        }
    }
}
