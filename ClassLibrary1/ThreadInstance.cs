using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class ThreadInstance
    {
        ThreadFactory threadFactory;
        public Thread assignedThread;

        public int entityId;

        public int _lockFlag = 0;

        Action<object> _threadWork;
        object obj;
        Action _threadOnCompleted;

        public ThreadInstance(ThreadFactory threadFactory)
        {
            this.threadFactory = threadFactory;
        }

        public void ReleaseTheThread()
        {
            Console.WriteLine($"Thread[{Thread.CurrentThread.ManagedThreadId}] is trying to unlock thread thread [{entityId}]");
            Interlocked.Decrement(ref _lockFlag);
            Monitor.Exit(this);
            threadFactory.ThreadReleased?.Invoke(this);
        }

        // 1) Set Work, in my case ill assign ReleaseTheThread as an OnCompleted Action
        public void SetWork(Action<object> Work, object obj, Action OnCompleted)
        {
            _threadWork = Work;
            this.obj = obj;
            _threadOnCompleted = OnCompleted;
        }
        // 2) Launch thread
        public void LaunchThread()
        {
            assignedThread = new Thread(ThreadStart);
            assignedThread.Start();
        }

        void ThreadStart()
        {
            _threadWork?.Invoke(obj);
            _threadOnCompleted.Invoke();
        }
    }
}
