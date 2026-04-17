using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Zad_1.Data
{
    internal class XmlLoader
    {        
        public XmlDocument Load(string path) {
            XmlDocument? doc = null;

            try
            {
                doc = new XmlDocument();
                doc.Load(path);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"I/O error: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission denied: {path}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            
            return doc;
        }

        public XmlNode? GetNode(XmlDocument doc, string name)
        {
            return doc?.SelectSingleNode(name);
        }

        public XmlNodeList? GetNodes(XmlDocument doc, string name)
        {
            return doc?.SelectNodes(name);
        }
    }
}
