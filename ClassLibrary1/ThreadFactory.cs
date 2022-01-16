using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    
    public class ThreadFactory
    {
        List<ThreadInstance> threads;

        public readonly int amountOfThreads;

        public Action<ThreadInstance> ThreadReleased;

        AutoResetEvent autoResetEvent;

        int searchDelay;

        public ThreadFactory(int amountOfTasks, int searchDelay)
        {
            this.searchDelay = searchDelay;
            ThreadReleased += OnThreadReleased;
            this.amountOfThreads = amountOfTasks;
            threads = new List<ThreadInstance>();

            for (int i = 1; i <= this.amountOfThreads; i++)
                threads.Add(new ThreadInstance(this, i));

            autoResetEvent = new AutoResetEvent(true);
        }

        public ThreadInstance GetFreeThread()
        {
            bool foundFreeThread = TryToGetFreeThread(out ThreadInstance threadInstance);
            if (foundFreeThread) return threadInstance;

            autoResetEvent.WaitOne();

            foundFreeThread = TryGetVacantThreadPersistent(out threadInstance);
            if (foundFreeThread) return threadInstance;
            else Console.WriteLine("Error, couldn't find free seller althought there's should be at least one");

            return null;
        }

        private bool TryToGetFreeThread(out ThreadInstance threadInstance)
        {
            threadInstance = null;
            foreach (var a in threads)
            {
                if (Interlocked.CompareExchange(ref a._lockFlag, 1, 0) == 0)
                {
                    Console.WriteLine($"Thread[{Thread.CurrentThread.ManagedThreadId}] is locking thread [{a.entityId}]");
                    //Monitor.Enter(a);
                    threadInstance = a;
                    return true;
                }
            }
            return false;
        }

        bool TryGetVacantThreadPersistent(out ThreadInstance threadInstance)
        {
            threadInstance = null;
            int iteration = 1;
            while (threadInstance == null)
            {
                foreach (var a in threads)
                {
                    if (Interlocked.CompareExchange(ref a._lockFlag, 1, 0) == 0)
                    {
                        //Monitor.Enter(a);
                        threadInstance = a;

                        Console.WriteLine($"Finished Persistent ThreadInstance aquasition on iteration [{iteration}], received thread [{threadInstance.assignedThread.ManagedThreadId}]");
                        return true;
                    }
                }
                iteration++;

                int chosenDelay = Math.Clamp(searchDelay, 1, 1000);
                Thread.Sleep(chosenDelay);
            }
            return false;
        }

        void OnThreadReleased(ThreadInstance threadInstance)
        {
            autoResetEvent.Set();
        }

    }
}
