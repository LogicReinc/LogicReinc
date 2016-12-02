using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LogicReinc.Threading
{
    public class WorkerPool<T>
    {
        public WorkerPool(int workerCount, int waitInterval)
        {
            WaitTaskInterval = waitInterval;
            AdjustWorkerCount(workerCount);
        }

        public int MaxThreads { get; private set; }
        public int WaitTaskInterval { get; set; } = 100;
        public ConcurrentQueue<WorkTask<T>> WorkQueue { get; private set; } = new ConcurrentQueue<WorkTask<T>>();
        private List<Worker<T>> Workers { get; set; } = new List<Worker<T>>();

        public void QueueWork(T obj, Action<T> work)
        {
            WorkQueue.Enqueue(new WorkTask<T>()
            {
                Data = obj,
                Work = work
            });
        }

        public void QueueWork(T obj, Action<T> work, Action<T> callback)
        {
            WorkQueue.Enqueue(new WorkTask<T>()
            {
                Data = obj,
                Work = work,
                Callback = callback
            });
        }

        public void SetWorkerCount(int newCap)
        {
            AdjustWorkerCount(newCap);
        }

        public void SetWorkerInterval(int newInterval)
        {
            WaitTaskInterval = newInterval;
            lock (Workers)
            {
                foreach (Worker<T> w in Workers)
                    w.WaitInterval = newInterval;
            }
        }

        private void AdjustWorkerCount(int newCap)
        {
            lock (Workers)
            {
                if (newCap < Workers.Count)
                    for (int i = 0; i < (Workers.Count - newCap); i++)
                        Workers[i].Stop();
                else
                {
                    List<Worker<T>> newWorkers = new List<Worker<T>>();
                    int newWorkerCap = (newCap - Workers.Count);
                    for (int i = 0; i < newWorkerCap; i++)
                    {
                        Worker<T> w = new Worker<T>(WorkQueue, WaitTaskInterval);
                        w.OnWorkCompleted += HandleWorkerCompleted;
                        w.OnWorkerStopped += HandleWorkerStopped;
                        w.OnWorkException += HandleWorkerException;
                        newWorkers.Add(w);
                        Workers.Add(w);
                    }
                    foreach (Worker<T> w in newWorkers)
                        w.Start();
                }
            }
        }

        private void HandleWorkerCompleted(WorkTask<T> obj)
        {
            if (obj.Callback != null)
                obj.Callback(obj.Data);
        }

        private void HandleWorkerException(WorkTask<T> obj, Exception ex)
        {
        }

        private void HandleWorkerStopped(Worker<T> w)
        {
            lock (Workers)
            {
                Workers.Remove(w);
            }
        }
    }
}