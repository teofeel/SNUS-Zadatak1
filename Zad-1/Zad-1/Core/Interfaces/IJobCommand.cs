using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zad_1.Models;

namespace Zad_1.Services.Interfaces
{
    internal abstract class IJobCommand
    {
		protected Job job;

		public Job Job
        {
			get { return job; }
			set { job = value; }
		}


        protected TaskCompletionSource<int> tsc;

		public TaskCompletionSource<int> TSC
		{
			get { return tsc; }
			set { tsc = value; }
		}

        protected IJobCommand(Job job, TaskCompletionSource<int> tsc)
        {
            this.job = job;
            this.tsc = tsc;
        }

        public abstract void execute();
	}
}
