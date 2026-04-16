using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zad_1.Models
{
    internal class JobHandle
    {
		private Guid id;

		public Guid Id
		{
			get { return id; }
			set { id = value; }
		}

		private Task<int> result;

		public Task<int> Result
		{
			get { return result; }
			set { result = value; }
		}

        public JobHandle(Guid id, Task<int> result)
        {
            this.id = id;
            this.result = result;
        }
    }
}
