using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Xml.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using System.Linq;
using System.Configuration;
using Microsoft.Azure;

namespace k190324_Q3
{
    public class Function1
    {
        [FunctionName("Function1")]
        public void Run([TimerTrigger("0 */20 * * * *")]TimerInfo myTimer, ILogger log)
        {
            convertXmltoJson C1 = new convertXmltoJson();

            string path = Environment.GetEnvironmentVariable("rootPath");
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now + path}");

            C1.performConversion(path);          
        }
    }

    public class convertXmltoJson
    {
        public void performConversion(string path)
        {
            string[] dirs = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);

            foreach (string dir in dirs)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    var fileCount = (from file in Directory.EnumerateFiles(dir + "/", " *.xml", SearchOption.AllDirectories)
                                     select file).Count();

                    string patmp = dir + "/" + dir.Remove(0, path.Length).ToLower() + " " + (fileCount).ToString() + ".xml";
                    if (!patmp.Contains("output"))
                    {
                        doc.Load(patmp);
                        XmlNodeList scriptNames = doc.GetElementsByTagName("Script");
                        XmlNodeList scriptPrice = doc.GetElementsByTagName("Price");

                        String GetTimestamp(DateTime value)
                        {
                            return value.ToString("yyyyMMddHHmmssffff");
                        }

                        for (int i = 0; i < scriptNames.Count; i++)
                        {
                            string[] chars = new string[] { ",", " ", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "(", ")", ":", "|", "[", "]" };
                            for (int cnt = 0; cnt < chars.Length; cnt++)
                            {
                                if (scriptNames[i].InnerXml.ToString().Contains(chars[cnt]))
                                    scriptNames[i].InnerXml = scriptNames[i].InnerXml.ToString().Replace(chars[cnt], "");
                            }
                            string filePath = path + "/Output/" + dir.Remove(0, path.Length) + "/" + scriptNames[i].InnerXml + ".json";

                            if (!Directory.Exists(filePath))
                            {
                                Directory.CreateDirectory(path + "/Output/" + dir.Remove(0, path.Length));
                            }
                            scriptData tmpObj = new scriptData();

                            if (!File.Exists(filePath))
                            {
                                Script tmpScript = new Script();

                                tmpScript.price = scriptPrice[i].InnerXml;
                                tmpScript.date = GetTimestamp(DateTime.Now);
                                tmpObj.scripts = new List<Script>();
                                tmpObj.scripts.Add(tmpScript);
                                tmpObj.lastUpdatedOn = GetTimestamp(DateTime.Now);
                            }
                            else
                            {                                                                      //if file doesn't exist
                                using (StreamReader file = File.OpenText(filePath))
                                {
                                    Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                                    tmpObj = (scriptData)serializer.Deserialize(file, typeof(scriptData));
                                }
                                Script tmpScript = new Script();

                                tmpScript.price = scriptPrice[i].InnerXml;
                                tmpScript.date = GetTimestamp(DateTime.Now);
                                tmpObj.scripts.Add(tmpScript);
                                tmpObj.lastUpdatedOn = GetTimestamp(DateTime.Now);
                            }

                            using (StreamWriter file = File.CreateText(filePath))
                            {
                                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                                serializer.Serialize(file, tmpObj);
                            }
                            File.Delete(patmp);
                        }
                    }
               }catch (Exception e) { }
            }
        }
    }
}
