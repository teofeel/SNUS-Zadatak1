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

		public int GetDelay()
		{
			if (payload == null || type.Equals(JobType.Prime))
				return -1;

			string timeStr = this.payload.Split(':')[1];

			return int.Parse(timeStr.Replace("_", ""));
		}


		public int GetNumbers()
		{
            if (payload == null || type.Equals(JobType.IO))
                return -1;

			string numbersStr = this.payload.Split(",")[0];
			string numberStr = numbersStr.Split(':')[1];

            return int.Parse(numberStr.Replace("_", ""));
        }

        public int GetThreads()
        {
            if (payload == null || type.Equals(JobType.IO))
                return -1;

            string threadsStr = this.payload.Split(",")[1];
            string threadStr = threadsStr.Split(':')[1];
            int num = int.Parse(threadStr);

			return num;
        }

		public override string ToString()
		{
			return "[ " + this.id + " | " +this.type.ToString() + " | " + this.payload + " | " + this.priority.ToString() + "]";
		}
    }
}
