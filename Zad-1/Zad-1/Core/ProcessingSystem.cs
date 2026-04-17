using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly object _recordsLock = new object();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);
        private List<JobRecord> records = new List<JobRecord>();

        public List<JobRecord> RecordsSnapshot
        {
            get {
                lock (_recordsLock)
                {
                    return new List<JobRecord>(records); 
                }
            }

        }

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
                _ = Task.Run(() => WorkerProcess());
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

        private async Task WorkerProcess()
        {
            while (true)
            {
                try
                {
                    await this._signal.WaitAsync();

                    IJobCommand job;

                    lock (_lock)
                    {
                        if (!this._queue.TryDequeue(out job, out _)) continue;
                    }

                    await Process(job);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Worker error: {ex.Message}");
                }
            }
        }
        private async Task Process(IJobCommand job) 
        {
            int retries = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();

            while (retries < 3)
            {
                retries++;

                try
                {
                    Task? executeTask = Task.Run(() =>  job.execute());
                    Task? timeoutTask = Task.Delay(2000);
                    Task? completedTask = await Task.WhenAny(executeTask, timeoutTask);

                    if(completedTask == executeTask)
                    {
                        await executeTask;
                        HandleCompleted(job, stopwatch);

                        return;
                    }
                    else
                    {
                        throw new TimeoutException($"Job {job.Job.Id} failed to complete");
                    }
                    
                }
                catch (Exception ex)
                {
                    if(retries < 3)
                    {
                        HandleFailure(job, stopwatch);
                        continue;
                    }
                    else if (retries >= 3)
                    {
                        HandleFailure(job, stopwatch);
                        job.TSC.TrySetException(ex);
                    }
                }
                
            }
        }

        private void HandleCompleted(IJobCommand job, Stopwatch stopwatch)
        {
            stopwatch.Stop();

            lock (_recordsLock)
            {
                this.records.Add(new JobRecord(job.Job.Id, job.Job.Type, true, stopwatch.Elapsed.TotalMilliseconds));
            }

            this.jobCompleted?.Invoke(this, new JobHandle(job.Job.Id, job.TSC.Task));
        }

        private void HandleFailure(IJobCommand job, Stopwatch stopwatch)
        {
            stopwatch.Stop();

            lock (_recordsLock)
            {
                this.records.Add(new JobRecord(job.Job.Id, job.Job.Type, false, stopwatch.Elapsed.TotalMilliseconds));
            }

            this.jobFailed?.Invoke(this, new JobHandle(job.Job.Id, job.TSC.Task));
        }

        public IEnumerable<Job> GetTopJobs(int n)
        {
            lock (this._lock)
            {
                return this._queue
                .UnorderedItems
                .OrderBy(x => x.Priority)
                .Take(n)
                .Select(x => x.Element.Job);
            }
           
        }

        public Job GetJob(Guid id)
        {
            lock (this._lock)
            {
                return this._queue
                .UnorderedItems
                .FirstOrDefault(x => x.Element.Job.Id == id)
                .Element.Job;
            }
        }

    }
}
