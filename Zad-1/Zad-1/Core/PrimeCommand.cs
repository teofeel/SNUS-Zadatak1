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
            int num = 0;
            object _lock = new object();
            List<Thread> threads = new List<Thread>();
            int total = this.job.GetNumbers();

            ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = Math.Clamp(this.job.GetThreads(), 1, 8) };

            Parallel.For(0, total, options, i =>
            {
                if (this.IsPrime(i))
                {
                    lock (_lock)
                    {
                        num++;
                    }
                }
            });

            this.tsc.TrySetResult(num);
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
