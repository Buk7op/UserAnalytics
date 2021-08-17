using System;
using System.Configuration;

namespace UserAnalytics
{
    class Program
    {
        
        static void Main(string[] args)
        {
            InformationCollector.CreateAnalytics();
            if(Convert.ToBoolean(ConfigurationManager.AppSettings.Get("Parent")))
            {
                FolderSynchronizer.Synchronize();
            }
        }

    }
}
