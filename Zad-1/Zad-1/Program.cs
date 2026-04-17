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
            try
            {
                if (args.Length < 1)
                    throw new ArgumentNullException("No args provided");

                SystemConfigurer configurer = new SystemConfigurer();
                Logger logger = new Logger();

                configurer.Initialize(args[0]);

                ProcessingSystem system = new ProcessingSystem(configurer.WorkerCount, configurer.MaxQueueSize);

                system.JobCompleted += async (sender, handle) =>
                {
                    int result = await handle.Result;
                    string message = $"[{DateTime.Now.ToString()}] [SUCCESS] {handle.Id}, {result}\n";

                    await logger.WriteLogAsync(message);
                };

                system.JobFailed += async (sender, handle) =>
                {
                    string message = $"[{DateTime.Now.ToString()}] [ABORT] {handle.Id}, N/A\n";

                    await logger.WriteLogAsync(message);
                };

                List<Job> jobs = configurer.LoadJobs();

                foreach (Job job in jobs)
                {
                    system.Submit(job);
                }

                JobProducer producer = new JobProducer();
                producer.ProduceAsync(configurer.WorkerCount, system);

                ReportGenerator reporter = new ReportGenerator();
                reporter.GenerateReportsAsync(system);


                Console.WriteLine("Jobs has been sent, to exit press x");

                Cli.Menu(system);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Make sure to send path to xml file");
            }
            catch(FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Make sure that path is valid");
            }
            catch(InvalidDataException ex)
            {
                Console.WriteLine("Data that must exist doesn't");
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}

