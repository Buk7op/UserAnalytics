using System;
using System.IO;
using System.Configuration;
using NLog;

namespace UserAnalytics
{
    class FolderManager
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        private int _howOld;
        private string _mainDirectory;
        public FolderManager()
        {
            _howOld = Int32.Parse(ConfigurationManager.AppSettings.Get("howOld"));
            _mainDirectory = ConfigurationManager.AppSettings.Get("dir");
        }


        public void DeleteOldDirectory()
        {
        logger.Trace("Удаляю старые директории");
        string[] dirList = Directory.GetDirectories(_mainDirectory);
        foreach(var d in dirList)
        {
            string[] internalDirList = Directory.GetDirectories(d);
            foreach(var dir in internalDirList)
            {
                if ((DateTime.Now - Directory.GetLastWriteTime(d)).Days > _howOld) 
                {
                    Directory.Delete(dir,true);
                }
            }
        }

        }
        
    }
}