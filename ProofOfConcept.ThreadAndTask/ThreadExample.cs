using System;
using System.Threading;

namespace ProofOfConcept.ThreadAndTask
{
    /// <summary>
    /// ThreadExample
    /// A Thread is a small set of executable instructions
    /// Document: https://docs.microsoft.com/en-us/dotnet/api/system.threading.thread?view=net-5.0  
    ///     Retrieving Thread objects
    ///     Foreground and background threads
    ///     Culture and threads
    /// </summary>
    public static class ThreadExample
    {
        // The ThreadProc method is called when the thread starts.
        // It loops ten times, writing to the console and yielding
        private static void ThreadProc()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("ThreadProc: {0}", i);
                // Yield the rest of the time slice.
                Thread.Sleep(1);
            }
        }

        private static void ThreadProc(object obj)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("ThreadProc: {0} {1}", obj.ToString(), i);
                // Yield the rest of the time slice.
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Run Thread
        /// The new thread (ThreadProc) run with main thread.
        /// </summary>
        public static void RunNewThread()
        {
            Console.WriteLine("Main thread: Start a second thread.");
            // The constructor for the Thread class requires a ThreadStart
            // delegate that represents the method to be executed on the
            // thread.  C# simplifies the creation of this delegate.
            Thread t = new Thread(new ThreadStart(ThreadProc));

            // Start ThreadProc.
            t.Start();

            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine("Main thread: Do some work.");
                Thread.Sleep(1);
            }

            Console.WriteLine("Main thread: Call Join(), to wait until ThreadProc ends.");
            t.Join();
            Console.WriteLine("Main thread: ThreadProc.Join has returned");
        }

        /// <summary>
        /// Run NewThread With Parameter
        /// The new thread (ThreadProc) with parameter run with main thread.
        /// </summary>
        public static void RunNewThreadWithParameter()
        {
            Console.WriteLine("Main thread: Start a second thread.");
            // The constructor for the Thread class requires a ThreadStart
            // delegate that represents the method to be executed on the
            // thread.  C# simplifies the creation of this delegate.
            Thread t = new Thread(new ParameterizedThreadStart(ThreadProc));

            // Start ThreadProc.
            t.Start("Thread with parameter");

            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine("Main thread: Do some work.");
                Thread.Sleep(1);
            }

            Console.WriteLine("Main thread: Call Join(), to wait until ThreadProc ends.");
            t.Join();
            Console.WriteLine("Main thread: ThreadProc.Join has returned.");
        }
    }
}
