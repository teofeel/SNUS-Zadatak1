using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zad_1.Models;

namespace Zad_1.Services
{
    internal class ProcessingSystem
    {
        private PriorityQueue<Job, int> _queue = new();
        private HashSet<Guid> _processedIds = new(); 
        private readonly object _lock = new object();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        private int _workerCount, _maxQueueSize;

        public ProcessingSystem(int workerCount, int maxQueueSize) 
        { 
            this._workerCount = workerCount;
            this._maxQueueSize = maxQueueSize;
        }

        public JobHandle Submit(Job job)
        {
            return null;
        }

        private async Task Process() {
        
        }

        private async Task<int> ProcessLogic(Job job)
        {
            // to implement
            return 0;
        }
    }
}
