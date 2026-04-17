using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Zad_1.Data;
using Zad_1.Enums;
using Zad_1.Models;

namespace Zad_1.Management
{
    internal class SystemConfigurer
    {
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


        public SystemConfigurer()
        {
            _loader = new XmlLoader();
        }

		public void Initialize(string path)
		{
            _doc = _loader.Load(path);

            if (this._doc == null)
                throw new FileNotFoundException("Document isnt loaded");

            LoadMetaData();
        }

        private void LoadMetaData()
        {
            XmlNode? countNode = _loader.GetNode(_doc, "//WorkerCount");
            XmlNode? queueSizeNode = _loader.GetNode(_doc, "//MaxQueueSize");

            if (countNode == null || queueSizeNode == null)
                throw new InvalidDataException("Could not load configurer file metada");

            workerCount = int.Parse(countNode.InnerText);
            maxQueueSize = int.Parse(queueSizeNode.InnerText);
        }

        public List<Job> LoadJobs()
        {
            List<Job> jobs = new List<Job>();

            XmlNodeList jobNodes = _loader.GetNodes(_doc, "//Jobs/Job");

            foreach (XmlNode node in jobNodes)
            {
                try
                {
                    Job job = CreateJob(node);
                    jobs.Add(job);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Job couldn't be created: {ex.Message}, continuing....");
                    continue;
                }
            }

            return jobs;

        }

        private Job CreateJob(XmlNode jobNode)
        {
            string typeStr = jobNode.Attributes["Type"]?.Value;
            string payload = jobNode.Attributes["Payload"]?.Value;
            string priorityStr = jobNode.Attributes["Priority"]?.Value;
            

            if (payload == null || payload == "") throw new InvalidDataException("Payload must exist");
            if (typeStr == null || typeStr == "") throw new InvalidDataException("Type must exist");
            if (priorityStr == null || priorityStr == "") throw new InvalidDataException("Priority must exist");

            int priority = int.Parse(priorityStr);

            if (Enum.TryParse(typeStr, out JobType typeEnum))
            {
                Job job = new Job(typeEnum, payload, priority);

                return job;
            }
            else
                throw new InvalidDataException("Job couldn'tbe created");
        }
	}
}
