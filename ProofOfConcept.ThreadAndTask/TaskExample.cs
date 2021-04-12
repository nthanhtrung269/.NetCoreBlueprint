using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProofOfConcept.ThreadAndTask
{
    /// <summary>
    /// TaskExample
    /// The Task class represents a single operation that does not return a value and that usually executes asynchronously
    /// Document: https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=net-5.0
    ///     Separating task creation and execution
    ///     Async and wait
    ///     CancellationTokenSource 
    /// </summary>
    public static class TaskExample
    {
        public static void CreateAndRunTask()
        {
            Action<object> action = (object obj) =>
            {
                Console.WriteLine("Task={0}, obj={1}, Thread={2}",
                Task.CurrentId, obj,
                Thread.CurrentThread.ManagedThreadId);
            };

            // Create a task but do not start it.
            Task t1 = new Task(action, "alpha");

            // Construct a started task
            Task t2 = Task.Factory.StartNew(action, "beta");

            // Launch t1 
            t1.Start();
            Console.WriteLine("t1 has been launched. (Main Thread={0})",
                              Thread.CurrentThread.ManagedThreadId);

            // Construct a started task using Task.Run.
            string taskData = "delta";
            Task t3 = Task.Run(() =>
            {
                Console.WriteLine("Task={0}, obj={1}, Thread={2}",
                                  Task.CurrentId, taskData,
                                   Thread.CurrentThread.ManagedThreadId);
            });

            // Construct an unstarted task
            Task t4 = new Task(action, "gamma");
            // Run it synchronously
            // Although the task was run synchronously, it is a good practice
            t4.RunSynchronously();
        }

        /// <summary>
        /// create a function return a task
        /// </summary>
        /// <returns>Task.</returns>
        private static async Task TaskOne()
        {
            await Task.Delay(2000);
            Console.WriteLine("TaskOne");
        }

        private static async Task<string> TaskTwo()
        {
            await Task.Delay(2000);
            Console.WriteLine("TaskTwo");
            return "TaskTwo";
        }

        /// <summary>
        /// create a function return a task{string}
        /// </summary>
        /// <returns>Task{string}.</returns>
        private static async Task<string> TaskThree(string name)
        {
            await Task.Delay(2000);
            Console.WriteLine("Hello" + name);
            return "Hello" + name;
        }

        public static async Task CreateAndRunTaskWithWait()
        {
            Console.WriteLine("Thead Vs Async/Await");

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            Task task_one = TaskOne();
            task_one.Wait();

            Task<string> task_two = TaskTwo();
            await task_two;

            await TaskThree("Task Three");

            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
        }

        public static async Task CreateAndRunTaskWithWhenAll()
        {
            Console.WriteLine("Thead Vs Async/Await and WhenAll");

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            Task task_one = TaskOne();

            Task<string> task_two = TaskTwo();

            Task<string> task_three = TaskThree("Task Three");

            await Task.WhenAll(task_one, task_two, task_three);

            Console.WriteLine($"Execution Time WhenAll: {watch.ElapsedMilliseconds} ms");
        }

        public static async Task CreateAndRunTaskWithResult()
        {
            Task<string> task = Task.Run(() =>
            {
                return "Result of task1.";
            });

            string result1 = task.Result;

            Console.WriteLine(result1);

            string result2 = await Task.Run(() =>
            {
                return "Result of task2.";
            });

            Console.WriteLine(result2);
        }
    }
}
