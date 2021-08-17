using System;
using System.Threading.Tasks;
using System.Configuration;
using Client.Domain.Contracts.NeosyntezUserServices;
using Client.Domain.Enums;
using Client.Domain.Models.NeosyntezObjects;
using Client.Domain.Contracts.WebApi;
using Client.Domain.Contracts.NeoSyntezApi;
using Client.Domain.Models.WorkObjects;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Collections.Generic;
using NLog;

namespace UserAnalytics
{
    public class nsObjectBrowser
        {
            private readonly IUserServiceUnitOfWork serviceUnitOfWork;
            private readonly IApiServiceUnitOfWork apiServiceUnitOfWork;


            public nsObjectBrowser(IUserServiceUnitOfWork serviceUnitOfWork, IApiServiceUnitOfWork apiServiceUnitOfWork)
            {
                this.serviceUnitOfWork = serviceUnitOfWork;
                this.apiServiceUnitOfWork = apiServiceUnitOfWork;
            } 
            
            Logger logger = LogManager.GetCurrentClassLogger();
           
            /// <summary>
            /// Пример получения объекта через API Неосинтез
            /// </summary>
            /// <returns>Объект из Неосинтез</returns>
            public NeosyntezObject GetObject(Guid guid)
            {
                return serviceUnitOfWork.UserObjectWork.GetNSObject(guid);
            }
            
            public void DeleteChilds(string guid)
            {
                var listOfObject = serviceUnitOfWork.UserObjectWork.GetChildGuids(Guid.Parse(guid));
               
                   foreach (var item in listOfObject)
                {
                    //if (GetObject(item).ModificationDate < DateTime.Today)
                    try
                    {
                    serviceUnitOfWork.UserObjectWork.DeleteObject(item);
                    } 
                    catch
                    {

                    }
                }
               
                
                
            }
            private NeosyntezFileContent LoadContent(string path,string fileName)
            {
                logger.Trace($"Загружаю файл {fileName} взятый из {path}");
                FileStream fstream = null;
                try 
                {
                    fstream = new FileStream(path, FileMode.Open);
                    BinaryFileObject a = new BinaryFileObject(fstream,fileName);
                    return apiServiceUnitOfWork.ContentApiService.Create(a);
                }
                finally
                {
                    if(fstream !=null) 
                    {
                        fstream.Close();
                    }
                }
                
            }

            public List<NeosyntezFileContent> LoadPackContent(Dictionary<string,string> filePath)
            {
                logger.Trace($"Связываю файлы с атрибутами");
                List<NeosyntezFileContent> packContent = new List<NeosyntezFileContent>();
                foreach (var item in filePath)
                {
                    logger.Trace($"Связываю {item.Key}/{item.Value}");
                    packContent.Add(LoadContent(item.Key,item.Value));
                }
                return packContent;
            }
            public IBinaryFileObject GetContent(Guid contentId)
            {
                return apiServiceUnitOfWork.ContentApiService.Get(contentId);
            }


            public Task AddNewObjectWithParentAndAttributeAsync(List<NeosyntezFileContent> fileContent,string name)
            {
                logger.Trace($"Создаю объект {name}");
                name = $"{name} от {DateTime.Now.ToShortDateString()}";
                Guid classId = Guid.Parse(ConfigurationManager.AppSettings.Get("parentClassId"));
                Guid parentId = Guid.Parse(ConfigurationManager.AppSettings.Get("parentId"));
                List<JToken> jTokens = CreateJtokenList(fileContent);
                List<NeosyntezAttribute> neosyntezAttributes = CreateAttributesList(jTokens);
                logger.Trace($"Объект {name} создан");
                return serviceUnitOfWork.UserObjectWork.CreateNewNSObjectAsync(parentId,name,classId,neosyntezAttributes);
            }



            private List<JToken> CreateJtokenList(List<NeosyntezFileContent> fileContent)
            {
                var options = new JsonSerializerOptions
                {
                    IgnoreNullValues = true
                };
                List<JToken> jTokens = new List<JToken>();
                foreach (var item in fileContent)
                {
                    string json = JsonSerializer.Serialize<NeosyntezFileContent>(item,options);
                    jTokens.Add(JToken.Parse(json));
                }
                return jTokens;
            }

            private List<NeosyntezAttribute> CreateAttributesList(List<JToken> jTokens)
            {
                logger.Trace($"Создаю атрибут UniqueUserList");
                var UniqueUserList = new NeosyntezAttribute
                {
                    Id = Guid.Parse(ConfigurationManager.AppSettings.Get("UniqueUserList")),
                    Name = "UniqueUserList",
                    Type = NeosyntezType.File,
                    Value = jTokens[0]
                };
                logger.Trace($"Создаю атрибут LastMonthUserList");
                var LastMonthUserList = new NeosyntezAttribute
                {
                    Id = Guid.Parse(ConfigurationManager.AppSettings.Get("LastMonthUserList")),
                    Name = "LastMonthUserList",
                    Type = NeosyntezType.File,
                    Value = jTokens[1]
                };
                logger.Trace($"Создаю атрибут Dashboard");
                var Dashboard = new NeosyntezAttribute
                {
                    Id = Guid.Parse(ConfigurationManager.AppSettings.Get("Summary")),
                    Name = "Dashboard",
                    Type = NeosyntezType.File,
                    Value = jTokens[2]
                };

                List<NeosyntezAttribute> neosyntezAttributes = new List<NeosyntezAttribute>();
                neosyntezAttributes.Add(Dashboard);
                neosyntezAttributes.Add(LastMonthUserList);
                neosyntezAttributes.Add(UniqueUserList);
                return neosyntezAttributes;
            }
         
            
        } 
}