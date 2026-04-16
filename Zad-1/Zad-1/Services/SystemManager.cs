using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Zad_1.Enums;
using Zad_1.Models;

namespace Zad_1.Services
{
    internal class SystemManager
    {
        private ProcessingSystem _processingSystem;

        private XmlLoader _loader;
        private XmlDocument _doc;

        private int workerCount;

        public int WorkerCount
        {
            get { return workerCount; }
            set { workerCount = value; }
        }


        private int maxQueueSize;

        public int MaxQueueSize
        {
            get { return maxQueueSize; }
            set { maxQueueSize = value; }
        }


        public SystemManager()
        {
            _loader = new XmlLoader();
            _processingSystem = new ProcessingSystem();
        }

		public void Initialize(string path)
		{
            this._doc = _loader.Load("C:/Users/teodo/Documents/FTN/SNUS/Zad1/SystemConfig/SystemConfig.xml");

            LoadMetaData();
        }

        private void LoadMetaData()
        {
            XmlNode countNode = _loader.GetNode(_doc, "//WorkerCount");
            XmlNode queueSizeNode = _loader.GetNode(_doc, "//MaxQueueSize");

            this.workerCount = int.Parse(countNode.InnerText);
            this.maxQueueSize = int.Parse(queueSizeNode.InnerText);
        }

        public List<Job> LoadJobsData()
        {
            List<Job> jobs = new List<Job>();

            XmlNodeList jobNodes = this._loader.GetNodes(_doc, "//Jobs/Job");

            foreach(XmlNode node in jobNodes)
            {
                Job job = CreateJob(node);
                if(job == null)
                {
                    Console.WriteLine("Job couldn't be created, continuing....");
                    continue;
                }

                jobs.Add(job);
            }

            return jobs;

        }

        private Job CreateJob(XmlNode jobNode)
        {
            string typeStr = jobNode.Attributes["Type"]?.Value;
            string payload = jobNode.Attributes["Payload"]?.Value;
            int priority = int.Parse(jobNode.Attributes["Priority"]?.Value ?? "10");


            if (Enum.TryParse(typeStr, out JobType typeEnum))
            {
                Job job = new Job(typeEnum, payload, priority);

                return job;
            }
            else
                return null;
        }
	}
}
