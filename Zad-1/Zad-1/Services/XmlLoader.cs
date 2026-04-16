using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Zad_1.Services
{
    internal class XmlLoader
    {        
        public XmlDocument Load(string path) { 
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                return doc;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public XmlNode GetNode(XmlDocument doc, string name)
        {
            return doc?.SelectSingleNode(name);
        }

        public XmlNodeList GetNodes(XmlDocument doc, string name)
        {
            return doc?.SelectNodes(name);
        }
    }
}
