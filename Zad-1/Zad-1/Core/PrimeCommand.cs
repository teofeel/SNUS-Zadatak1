using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zad_1.Models;
using Zad_1.Services.Interfaces;

namespace Zad_1.Services
{
    internal class PrimeCommand : IJobCommand
    {
        public PrimeCommand(Job job, TaskCompletionSource<int> tsc) : base(job, tsc)
        {
        }

        public override void execute()
        {
            object _lock = new object();
            List<Thread> threads = new List<Thread>();

            HashSet<int> primes = new HashSet<int>();
            int threadNum = Math.Clamp(this.job.GetThreads(), 1, 8);

            int totalNumbers = this.job.GetNumbers();
            int range = totalNumbers / threadNum;

            for (int i = 0; i < threadNum; i++)
            {
                int start = i * range;
                int end = (i == threadNum - 1) ? totalNumbers : (i + 1) * range;

                Thread t = new Thread(() => { 
                    for(int i = start; i < end; i++)
                    {
                        if (this.IsPrime(i))
                        {
                            lock (_lock)
                            {
                                primes.Add(i);
                            }
                        }
                    }
                });

                threads.Add(t);
                t.Start();
            }

            foreach (var t in threads)
            {
                t.Join();
            }

            this.tsc.TrySetResult(primes.Count);
        }

        private bool IsPrime(int n)
        {
            if (n <= 1) return false;

            if (n == 2) return true;

            if (n % 2 == 0) return false;

            for (int i = 3; i * i <= n; i += 2)
            {
                if (n % i == 0) return false;
            }

            return true;
        }

    }
}
