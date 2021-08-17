using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using NLog;

namespace UserAnalytics
{
    class FolderSynchronizer
    {
        
        public static void Synchronize()
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            List<string> childsDir = new List<string>();
            int childCount = Int32.Parse(ConfigurationManager.AppSettings.Get("childCount"));
            string parentPath = ConfigurationManager.AppSettings.Get("parentDir");
            
            for(int i = 1; i <= childCount; i++)
            {
                childsDir.Add(ConfigurationManager.AppSettings.Get($"childDir{i}"));
            }
            try
            {
                foreach (var item in childsDir)
                {
                    CopyFiles(item,parentPath,item);
                }
            }
            catch(Exception ex)
            {
                logger.Trace($"Произошло исключение {ex.Message}");
            }
            
        }

        public static void CopyFiles(string path,string parentPath,string childPath)
        {
             Logger logger = LogManager.GetCurrentClassLogger();
             logger.Trace($"Копирую {childPath}");
            string[] files = Directory.GetFiles(path);
            if (files.Length == 0)
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (var dir in dirs)
                {
                    CopyFiles(dir, parentPath,childPath);
                }
            }
            else
            {
                foreach (var file in files)
                { 
                    string destFile = file.Replace(childPath,parentPath);
                    string destPath = file.Replace(childPath,parentPath).Substring(0,file.LastIndexOf(@"\"));
                    if(!Directory.Exists(destPath))
                        Directory.CreateDirectory(destPath);    
                    File.Copy(file,destFile,true);
                }
            }
        }
    }
}