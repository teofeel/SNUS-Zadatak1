using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zad_1.Enums;

namespace Zad_1.Models
{
    internal class Job
    {
		private Guid id;

		public Guid Id	
		{
			get { return id; }
			set { id = value; }
		}

		private JobType type;

		public JobType Type
		{
			get { return type; }
			set { type = value; }
		}


		private string payload;

		public string Payload
		{
			get { return payload; }
			set { payload = value; }
		}

		private int priority;

		public int Priority
		{
			get { return priority; }
			set { priority = value; }
		}

		public Job(JobType type, string payload, int priority)
		{
			this.Id = Guid.NewGuid();
			this.type = type;
			this.payload = payload;
			this.priority = priority;
		}

	}
}
