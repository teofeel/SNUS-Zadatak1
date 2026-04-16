using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Zad_1.Models;
using Zad_1.Services;

namespace Zad_1
{
    class Program
    {
        static void Main(string[] args)
        {
            SystemManager manager = new SystemManager();

            manager.Initialize("C:/Users/teodo/Documents/FTN/SNUS/Zad1/SystemConfig/SystemConfig.xml");

            ProcessingSystem system = new ProcessingSystem(manager.WorkerCount, manager.MaxQueueSize);

            List<Job> jobs = manager.LoadJobsData();

            foreach(Job job in jobs)
            {
                system.Submit(job);
            }
        }

    }
}

