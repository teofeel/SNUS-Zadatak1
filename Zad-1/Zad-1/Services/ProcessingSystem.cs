using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zad_1.Enums;
using Zad_1.Models;
using Zad_1.Services.Interfaces;

namespace Zad_1.Services
{
    internal class ProcessingSystem
    {
        private PriorityQueue<IJobCommand, int> _queue = new();
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
            var tsc = new TaskCompletionSource<int>();
            IJobCommand command;

            if (job.Type.Equals(JobType.Prime)) command = new PrimeCommand(job, tsc);
            else command = new IOCommand(job, tsc);

            lock (_lock)
            {
                if (this._processedIds.Contains(job.Id))
                    return null;

                if (this._queue.Count > this._maxQueueSize)
                    return null;


                this._queue.Enqueue(command, job.Priority);
                this._processedIds.Add(job.Id);
            }

            this._signal.Release();

            return new JobHandle(job.Id, tsc.Task);
            
        }

        private void WorkerLoop()
        {
            while (true)
            {
                this._signal.Wait();

                IJobCommand job;

                lock (_lock)
                {
                    if (!this._queue.TryDequeue(out job, out _)) continue;
                }

                // call method to process this job, and put result back in TaskCompletionSource
               _ = Process(job);
            }
        }
        private async Task Process(IJobCommand job) 
        {
            int retries = 0;

            while( retries < 3)
            {

                retries++;
                try
                {
                    _ = Task.Run(() =>  job.execute());

                    var timeoutTask = Task.Delay(2000);
                    var completedTask = await Task.WhenAny(job.TSC.Task, timeoutTask);

                    if(completedTask == job.TSC.Task)
                    {
                        await job.TSC.Task;

                        this.jobCompleted?.Invoke(this, new JobHandle(job.Job.Id, job.TSC.Task));
                        return;
                    }
                    else
                    {
                        if (retries < 3) continue;
                        throw new TimeoutException($"Job {job.Job.Id} failed to complete");
                    }
                    
                }
                catch (Exception ex)
                {
                    if (retries >= 3)
                    {
                        job.TSC.TrySetException(ex);
                        this.jobFailed?.Invoke(this, new JobHandle(job.Job.Id, job.TSC.Task));
                    }
                }
                
            }
        }

    }
}
