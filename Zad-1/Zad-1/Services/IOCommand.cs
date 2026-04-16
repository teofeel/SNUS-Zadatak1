using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zad_1.Models;
using Zad_1.Services.Interfaces;

namespace Zad_1.Services
{
    internal class IOCommand : IJobCommand
    {
        public IOCommand(Job job, TaskCompletionSource<int> tsc) : base(job, tsc)
        {
        }

        public override void execute()
        {
            Thread.Sleep(Job.GetDelay());

            Random rnd = new Random();
            tsc.TrySetResult(rnd.Next(0, 101));
        }
    }
}
