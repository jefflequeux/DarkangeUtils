using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeosSdiConfiguration.Controls.Helpers;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;

namespace NeosSdiConfiguration
{
    [Serializable]
    public class ConfigurationSettings
    {
        public ObservableCollection<CodingRule> CodingRules { get; set; }
        protected System.Environment.SpecialFolder m_specialFolder = Environment.SpecialFolder.CommonApplicationData;
        protected string appFolder = "NeosSdiVisualStudio";
        protected string m_fileName = "neosConfigurationSettings.config";
        protected string m_dataFilePath = "";

        public ConfigurationSettings()
        {
            CodingRules = new ObservableCollection<CodingRule>();
            m_dataFilePath = CreateAppDataFolder(appFolder);
        }

        public void Save()
        {

            string fileName = System.IO.Path.Combine(m_dataFilePath, m_fileName);

            XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationSettings));
            TextWriter textWriter = new StreamWriter(fileName);
            serializer.Serialize(textWriter, this);
            textWriter.Close();
        }

        public ConfigurationSettings Load()
        {

            string fileName = System.IO.Path.Combine(m_dataFilePath, m_fileName);
            if (!File.Exists(fileName))
                return null;
            XmlSerializer deserializer = new XmlSerializer(typeof(ConfigurationSettings));
            TextReader textReader = new StreamReader(fileName);
            ConfigurationSettings newObj = (ConfigurationSettings)deserializer.Deserialize(textReader);
            textReader.Close();
            return newObj;
        }

        protected string CreateAppDataFolder(string folderName)
        {
            string appDataPath = "";
            string dataFilePath = "";

            folderName = folderName.Trim();
            if (folderName != "")
            {
                try
                {
                    // Set the directory where the file will come from.  The folder name 
                    // returned will be different between XP and Vista. Under XP, the default 
                    // folder name is "C:\Documents and Settings\All Users\Application Data\[folderName]"
                    // while under Vista, the folder name is "C:\Program Data\[folderName]".
                    appDataPath = System.Environment.GetFolderPath(m_specialFolder);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                // make sure w're only going to create a top-level folder
                if (folderName.Contains("\\"))
                {
                    string[] path = folderName.Split('\\');
                    int folderCount = 0;
                    int folderIndex = -1;
                    for (int i = 0; i < path.Length; i++)
                    {
                        string folder = path[i];
                        if (folder != "")
                        {
                            if (folderIndex == -1)
                            {
                                folderIndex = i;
                            }
                            folderCount++;
                        }
                    }
                    if (folderCount != 1)
                    {
                        throw new Exception("Invalid folder name specified (this function" +
                                            "only creates the root app data folder for the" +
                                            " application).");
                    }
                    folderName = path[folderIndex];
                }
            }
            if (folderName == "")
            {
                throw new Exception("Processed folder name resulted in an empty string.");
            }
            try
            {
                dataFilePath = System.IO.Path.Combine(appDataPath, folderName);
                if (!Directory.Exists(dataFilePath))
                {
                    Directory.CreateDirectory(dataFilePath);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataFilePath;
        }
    }
}
