using Client.Domain.Contracts.NeosyntezConfiguration;

using System;

namespace UserAnalytics
{
    /// <summary>
    /// Класс данных для подключения к экземпляру Неосинтез
    /// </summary>
    public class NeosyntezConfiguration : INeosyntezConfiguration
    {
        public Uri BaseAddress { get; set; } 
        public string ClientName { get ; set; }
        public string Secret { get ; set ; }
        public string UserName { get ; set ; }
        public string UserPassword { get ; set ; }
    }
}
