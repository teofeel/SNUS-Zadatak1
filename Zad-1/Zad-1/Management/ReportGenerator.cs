using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Zad_1.Enums;
using Zad_1.Models;
using Zad_1.Services;

namespace Zad_1.Management
{
    internal class ReportGenerator
    {
        private static int _reportIndex = 0;
        private readonly object _lock = new object(); 
        public void GenerateReportsAsync(ProcessingSystem system, CancellationToken token)
        {
            object _lock = new object();
            List<JobRecord> history = new List<JobRecord>();

            _ = Task.Run(async () => {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(60000);

                    lock (_lock)
                    {
                        history = system.RecordsSnapshot;

                        if (history.Any()) GenerateJobReport(history);
                    }
                }
            }, token);
        }


        public void GenerateJobReport(List<JobRecord> records)
        {
            try
            {
                var countByType = records
                .Where(r => r.Success)
                .GroupBy(r => r.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() });

                var averageTime = records
                    .GroupBy(r => r.Type)
                    .Select(g => new { Type = g.Key, Time = g.Average(r => r.ExecutionTime) });

                var failedByType = records
                    .Where(r => !r.Success)
                    .GroupBy(r => r.Type)
                    .OrderBy(g => g.Key)
                    .Select(g => new { Type = g.Key, Count = g.Count() });


                var doc = new XmlDocument();
                var root = doc.CreateElement("Report");
                doc.AppendChild(root);

                var timeAttr = doc.CreateAttribute("GeneratedAt");
                timeAttr.Value = DateTime.Now.ToString();
                root.Attributes.Append(timeAttr);


                var completedNode = doc.CreateElement("CompletedByType");
                foreach (var item in countByType)
                {
                    var node = doc.CreateElement("Entry");
                    node.SetAttribute("Type", item.Type.ToString());
                    node.SetAttribute("Count", item.Count.ToString());
                    completedNode.AppendChild(node);
                }
                root.AppendChild(completedNode);


                var avgNode = doc.CreateElement("AvgExecutionTimeByType");
                foreach (var item in averageTime)
                {
                    var node = doc.CreateElement("Entry");
                    node.SetAttribute("Type", item.Type.ToString());
                    node.SetAttribute("AvgMs", item.Time.ToString("F2"));
                    avgNode.AppendChild(node);
                }
                root.AppendChild(avgNode);

                var failedNode = doc.CreateElement("FailedByType");
                foreach (var item in failedByType)
                {
                    var node = doc.CreateElement("Entry");
                    node.SetAttribute("Type", item.Type.ToString());
                    node.SetAttribute("Count", item.Count.ToString());
                    failedNode.AppendChild(node);
                }
                root.AppendChild(failedNode);

                string fileName = $"report_{_reportIndex % 10}.xml";
                _reportIndex++;

                doc.Save(fileName);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"I/O error: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission denied: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}
