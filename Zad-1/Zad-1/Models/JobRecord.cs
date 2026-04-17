using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zad_1.Enums;

namespace Zad_1.Models
{
    internal class JobRecord
    {
		private Guid id;

		public Guid Id
		{
			get { return id; }
			set { id = value; }
		}

		private JobType	type;

		public JobType Type
		{
			get { return type; }
			set { type = value; }
		}

		private bool success;

		public bool Success
        {
			get { return success; }
			set { success = value; }
		}

		private double executionTime;

		public double ExecutionTime
        {
			get { return executionTime; }
			set { executionTime = value; }
		}

        public JobRecord(Guid id, JobType type, bool success, double executionTime)
        {
            this.id = id;
            this.type = type;
            this.success = success;
            this.executionTime = executionTime;
        }
    }
}
