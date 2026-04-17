using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Zad_1.Data;
using Zad_1.Models;
using Zad_1.Services;

namespace Zad_1
{
    class Program
    {
        static void Main(string[] args)
        {
            SystemConfigurer configurer = new SystemConfigurer();
            Logger logger = new Logger();

            configurer.Initialize("C:/Users/teodo/Documents/FTN/SNUS/Zad1/SystemConfig/SystemConfig.xml");

            ProcessingSystem system = new ProcessingSystem(configurer.WorkerCount, configurer.MaxQueueSize);

            system.JobCompleted += async (sender, handle) =>
            {
                Console.WriteLine($"Writing job {handle.Id} to file....");

                string message = $"[{DateTime.Now.ToString()}] [SUCCESS] {handle.Id} {handle.Result.Result}\n";
                await logger.WriteLogAsync(message);
            };

            system.JobFailed += async (sender, handle) =>
            {
                Console.WriteLine($"Writing job {handle.Id} to file....");

                string message = $"[{DateTime.Now.ToString()}] [ABORT] {handle.Id} N/A\n";
                await logger.WriteLogAsync(message);
            };

            List<Job> jobs = configurer.LoadJobs();
            foreach(Job job in jobs)
            {
                system.Submit(job);
            }


            Console.WriteLine("Jobs has been sent, to finish press any key");
            Console.ReadLine();
        }

    }
}

