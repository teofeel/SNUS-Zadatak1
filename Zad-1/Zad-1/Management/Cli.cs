using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zad_1.Models;
using Zad_1.Services;

namespace Zad_1.Management
{
    internal class Cli
    {
        public static void Menu(ProcessingSystem system)
        {
            string choice = "";
            while (choice != "x")
            {

                Console.WriteLine("To see more info: ");
                Console.WriteLine("1. Get top jobs");
                Console.WriteLine("2. Get job by id");

                Console.Write(">>> ");

                choice = Console.ReadLine().ToLower();

                // to do
                if (choice == "x")
                {
                    break;
                }

                MakeChoice(system, choice);

            }
        }

        private static void MakeChoice(ProcessingSystem system, string choice)
        {
            switch (choice)
            {
                case "1":
                    GetTopJobs(system);
                    break;
                case "2":
                    GetJobById(system);
                    break;
                default:
                    Console.WriteLine("This option doesn't exist");
                    break;
            }
        }

        private static void GetTopJobs(ProcessingSystem system)
        {
            Console.WriteLine("How many jobs would you like to see (enter number)");
            Console.Write(">>> ");
            string numStr = Console.ReadLine();

            try
            {
                int num = int.Parse(numStr);

                List<Job> jobs = system.GetTopJobs(num).ToList();

                foreach (Job job in jobs)
                {
                    Console.WriteLine(job.ToString());
                }
            }catch(Exception ex)
            {
                Console.WriteLine("You haven't entered number");
            }
        }

        private static void GetJobById(ProcessingSystem system)
        {
            Console.WriteLine("Which job do you want to get (enter Guid)");
            Console.Write(">>> ");
            string guidStr = Console.ReadLine();

            try
            {
                Guid id = Guid.Parse(guidStr);

                Job job = system.GetJob(id);

                Console.WriteLine(job.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("You haven't entered guid");
            }
        }
    }
}
