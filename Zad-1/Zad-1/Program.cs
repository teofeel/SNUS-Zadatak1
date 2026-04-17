using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Zad_1.Data;
using Zad_1.Management;
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
                int result = await handle.Result;
                string message = $"[{DateTime.Now.ToString()}] [SUCCESS] {handle.Id} {result}\n";

                await logger.WriteLogAsync(message);
            };

            system.JobFailed += async (sender, handle) =>
            {
                string message = $"[{DateTime.Now.ToString()}] [ABORT] {handle.Id} N/A\n";

                await logger.WriteLogAsync(message);
            };

            List<Job> jobs = configurer.LoadJobs();
            foreach(Job job in jobs)
            {
                system.Submit(job);
            }

            List<JobRecord> history = new List<JobRecord>();
            ReportGenerator reporter = new ReportGenerator();

            _ = Task.Run(async () => {
                while (true)
                {
                    await Task.Delay(60000);
                    lock (history)
                    {
                        history = system.RecordsSnapshot;

                        if (history.Any())
                        {
                            reporter.GenerateJobReport(history);
                        }
                    }
                }
            });

            Console.WriteLine("Jobs has been sent, to exit press x");

            string choice = "";
            while (choice != "x")
            {
                
                Console.WriteLine("To see more info: ");
                Console.WriteLine("1. Get top jobs");
                Console.WriteLine("2. Get job by id");

                Console.Write(">>> ");

                choice = Console.ReadLine().ToLower();

                // to do
            }
            
        }

    }
}

