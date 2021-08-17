using System;
using System.Configuration;
using System.Collections.Generic;
using NLog;
using Client.Domain.Contracts.NeosyntezUserServices;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Client.Domain.Models.NeosyntezObjects;
using Client.Domain.Contracts.NeoSyntezApi;

namespace UserAnalytics
{
    class InformationCollector
    {
        
        public static void CreateAnalytics()
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            FolderManager f = new FolderManager();
            DataWriter dataWriter = new DataWriter();
            //List<Connector> connectors = CreateConnectors();
            Sorter sorter = new Sorter();
            NeosyntezConnection nsConn = new NeosyntezConnection();
            try
            {
                ServiceProvider provider = nsConn.Connect().BuildServiceProvider();
                nsObjectBrowser nsObjectBrowser = new nsObjectBrowser(provider.GetService<IUserServiceUnitOfWork>(), provider.GetService<IApiServiceUnitOfWork>());
                //nsObjectBrowser.DeleteChilds(ConfigurationManager.AppSettings.Get("parentId"));
                nsObjectBrowser.DeleteChilds("43f742b2-c45b-11eb-a2f7-00505692b02f");
                DashboardCreator.ClearTable();
                // foreach(var c in connectors)
                // {
                //     if (c.GetDataSet() != null)
                //     {
                //         dataWriter.WriteUserList(c.GetDataSet(),c.GetCatalog());
                //         dataWriter.WriteUniqueUserList(sorter.FindUnique(c.GetDataSet()),c.GetCatalog());
                //         dataWriter.WriteLastMonthUserList(sorter.LastMonthUsers(c.GetDataSet()),c.GetCatalog());
                //         dataWriter.CreateExcel(c.GetDataSet(),sorter.FindUnique(c.GetDataSet()),c.GetCatalog());
                //         List<NeosyntezFileContent> fileContent = nsObjectBrowser.LoadPackContent(dataWriter.GetFilePath());
                //         Task a = nsObjectBrowser.AddNewObjectWithParentAndAttributeAsync(fileContent,c.GetCatalog());
                //         Task.Run(()=> a);
                //         Task.WaitAny(a);
                //         dataWriter.ClearFilePath();
                //     } 
                // }
            f.DeleteOldDirectory();
            }
            catch(Exception ex)
            {
                logger.Trace("Ошибка при создании провайдера: " + ex.Message);
            }
            
            
        }
        public static List<Connector> CreateConnectors()
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            List<Connector> connectors = new List<Connector>();
            try
            {
                for(int i = 1; i <= Int32.Parse(ConfigurationManager.AppSettings.Get("dataSourceCount"));i++) 
                {
                    string key1 = "dataSource" + i;
                    string key2 = "initCatalog" + i;
                    connectors.Add(new Connector(ConfigurationManager.AppSettings.Get(key1),ConfigurationManager.AppSettings.Get(key2)));
                }
            }
            catch(Exception ex)
            {
                logger.Error($"Неправильный config файл : {ex.ToString()}");
            }
            return connectors;
        }
    }
}