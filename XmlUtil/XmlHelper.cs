using LogLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlUtil
{
    public class XmlHelper
    {
        public static Dictionary<string, Dictionary<string, string>>  Read( string xmlPath,string name,string key,string value)
        {
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            if (File.Exists(xmlPath))
            {
               
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlPath);
                XmlElement rootElem = doc.DocumentElement;
                XmlNodeList personNodes = rootElem.GetElementsByTagName(name); //获取person子节点集合
                Dictionary<string, string> item = new Dictionary<string, string>();
                for (int i = 0; i < personNodes.Count; i++)
                {
                    item.Add(((XmlElement)personNodes.Item(i)).GetAttribute(key), ((XmlElement)personNodes.Item(i)).GetAttribute(value));
                }
                dic.Add(name, item);
            }
            else
            {
                Log.WriteLog("config file not exist:"+xmlPath);
            }
            return dic;
        }
    }
}
