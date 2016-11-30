using System;
using System.Collections.Concurrent;
using System.Threading;

namespace LogicReinc.Threading
{
    public class Worker<T>
    {
        public Worker(ConcurrentQueue<WorkTask<T>> tasks, int waitInterval)
        {
            TaskQueue = tasks;
            this.WaitInterval = waitInterval;
            Thread = new Thread(() =>
            {
                while (!Stopped)
                {
                    if (!TaskQueue.IsEmpty)
                    {
                        try
                        {
                            HandleWork();
                        }
                        catch (Exception ex)
                        {
                            //Iknow.
                            Console.WriteLine("Error on Worker: " + ex.Message);
                        }
                    }
                    else
                        Thread.Sleep(waitInterval);
                }
                if (OnWorkerStopped != null)
                    OnWorkerStopped(this);
            });
        }

        public delegate void OnWorkerCompletedDelegate(WorkTask<T> obj);

        public delegate void OnWorkerExceptionDelegate(WorkTask<T> obj, Exception ex);

        public delegate void OnWorkerStoppedDelegate(Worker<T> worker);

        public event OnWorkerCompletedDelegate OnWorkCompleted;

        public event OnWorkerStoppedDelegate OnWorkerStopped;

        public event OnWorkerExceptionDelegate OnWorkException;

        public bool Stopped { get; set; }
        public Thread Thread { get; private set; }
        public int WaitInterval { get; set; }
        private ConcurrentQueue<WorkTask<T>> TaskQueue { get; set; }

        //private WorkTask<T> Task { get; set; }
        public void HandleWork()
        {
            WorkTask<T> task;
            if (TaskQueue.TryDequeue(out task))
            {
                try
                {
                    task.Work(task.Data);
                }
                catch (Exception ex)
                {
                    if (OnWorkException != null)
                        OnWorkException(task, ex);
                }

                if (OnWorkCompleted != null)
                    OnWorkCompleted(task);
            }
        }

        public void Start()
        {
            Thread.Start();
        }

        public void Stop()
        {
            Stopped = true;
        }
    }
}