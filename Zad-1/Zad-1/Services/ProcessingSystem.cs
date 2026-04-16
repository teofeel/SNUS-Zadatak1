using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zad_1.Models;

namespace Zad_1.Services
{
    internal class ProcessingSystem
    {
        private PriorityQueue<(Job job, TaskCompletionSource<int> tsc), int> _queue = new();
        private HashSet<Guid> _processedIds = new(); 
        private readonly object _lock = new object();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        private List<Thread> _workers = new();

        private int _workerCount, _maxQueueSize;

        private EventHandler<JobHandle>? jobCompleted;

        public EventHandler<JobHandle>? JobCompleted
        {
            get { return jobCompleted; }
            set { jobCompleted = value; }
        }

        private EventHandler<JobHandle>? jobFailed;

        public EventHandler<JobHandle>? JobFailed
        {
            get { return jobFailed; }
            set { jobFailed = value; }
        }

        public ProcessingSystem(int workerCount, int maxQueueSize) 
        { 
            this._workerCount = workerCount;
            this._maxQueueSize = maxQueueSize;

            for (int i = 0; i < workerCount; i++)
            {
                Thread t = new Thread(WorkerLoop) { IsBackground = true };
                this._workers.Add(t);

                t.Start();
            }
        }

        public JobHandle? Submit(Job job)
        {
            lock (_lock)
            {
                if (this._processedIds.Contains(job.Id))
                    return null;

                if (this._queue.Count > this._maxQueueSize)
                    return null;

                var tsc = new TaskCompletionSource<int>();

                this._queue.Enqueue((job, tsc), job.Priority);
                this._processedIds.Add(job.Id);

                this._signal.Release();

                return new JobHandle(job.Id, tsc.Task);
            }
        }

        private void WorkerLoop()
        {
            while (true)
            {
                this._signal.Wait();

                (Job job, TaskCompletionSource<int> tsc) item;

                lock (_lock)
                {
                    if (!this._queue.TryDequeue(out item, out _)) continue;
                }

                // call method to process this job, and put result back in TaskCompletionSource

            }
        }
        private async Task Process(Job job, TaskCompletionSource tsc) 
        {
            // to do
        }

        private async Task<int> ProcessPrime()
        {
            // to do
            return -1;
        }

        private async Task<int> ProcessIO()
        {
            // to do
            return -1;
        }
    }
}
