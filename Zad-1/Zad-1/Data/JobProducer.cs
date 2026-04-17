using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Zad_1.Enums;
using Zad_1.Models;
using Zad_1.Services;

namespace Zad_1.Data
{
    internal class JobProducer
    {
        private static readonly Random _rand = new Random();
        private CancellationTokenSource _cts;


        public void ProduceAsync(int numThreads, ProcessingSystem system, CancellationToken token)
        {
            for (int i = 0; i < numThreads; i++)
            {
                _ = Task.Run(async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        system.Submit(Produce());
                        await Task.Delay(_rand.Next(1000, 5000), token);
                    }
                }, token);
            }
        }


        public Job Produce()
        {
            Job job;

            int typeCoinFlip = _rand.Next(0,2);

            if (typeCoinFlip == 0) job = CreatePrimeJob();
            else job = CreateIOJob();

            return job;
        }


        public Job CreateIOJob()
        {
            int delay = _rand.Next(1001, 50001);
            string payload = $"delay:{delay}";

            int priority = _rand.Next(1, 11);

            return new Job(JobType.IO, payload, priority);
        }


        public Job CreatePrimeJob()
        {
            int numbers = _rand.Next(10001, 50001);
            int threads = _rand.Next(1, 9);

            string payload = $"numbers:{numbers},threads:{threads}";

            int priority = _rand.Next(1, 11);

            return new Job(JobType.Prime, payload, priority);
        }
    }
}
