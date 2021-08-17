using Microsoft.Extensions.DependencyInjection;
using Client.Domain.Contracts.NeoSyntezApi;
using Client.Domain.Contracts.NeosyntezUserServices;
using Client.Domain.Contracts.WebApi;
using Client.Application.WebApi;
using Client.Domain.Contracts.NeosyntezConfiguration;
using Client.Application.Services.NeosyntezUserServices;
using Client.Application.Services.NeosyntezApiServices;
using System;
using NLog;
using System.Configuration;
namespace UserAnalytics
{
   public class NeosyntezConnection 
   {
        Logger logger = LogManager.GetCurrentClassLogger();
        public ServiceCollection Connect()
       {
           logger.Trace($"Конфигурирую подключение к Neosyntez");
           try
           {
                var services = new ServiceCollection();
                services.AddHttpClient();
                services.AddTransient<IWebRequestService, WebRequestService>();
                services.AddTransient<INeosyntezConfiguration>((IServiceProvider x) =>
                    {
                        NeosyntezConfiguration nsconf = new NeosyntezConfiguration();
                        nsconf.BaseAddress =new Uri(ConfigurationManager.AppSettings.Get("BaseAddress")); //адрес
                        nsconf.ClientName = ConfigurationManager.AppSettings.Get("ClientName"); // Идентификатор клиента
                        nsconf.Secret = ConfigurationManager.AppSettings.Get("Secret"); // Секретный ключ клиента 
                        nsconf.UserName = ConfigurationManager.AppSettings.Get("UserName"); // логин
                        nsconf.UserPassword = ConfigurationManager.AppSettings.Get("UserPassword"); // пароль 
                        return nsconf;
                    });

                services.AddTransient<IUserServiceUnitOfWork, UserServiceUnitOfWork>();
                services.AddTransient<IApiServiceUnitOfWork, ApiServiceUnitOfWork>();
                ServiceProvider provider = services.BuildServiceProvider();
                return services;
           }
           catch(Exception ex)
           {
               logger.Trace(ex.Message);
               return null;
           }
           
       }
   }
}