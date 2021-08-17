using System;
using System.Data;
using System.Data.SqlClient;
using NLog;

namespace UserAnalytics
{
    class Connector
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        private string _dataSource;
        private string _initCatalog;

        private DataSet _dataSet;
        public Connector(string dataSource, string initCatalog)
        {
            _dataSource = dataSource;
            _initCatalog = initCatalog; 
            logger.Trace($"Выполняю подключение к базе данных : {_dataSource} в каталог {_initCatalog}");
            string connectionString = $"Data source = {_dataSource}; Initial Catalog = {_initCatalog}; Integrated Security=SSPI;";
            string sql = $@"SELECT --x1.[id]
                        --,x1.[idAudit]
                            convert(varchar, x3.timestamp, 104)  timestamp
                        --,x1.[eventType]
                            ,x2.name EventTypeName
                        --,x1.[entityType]
                        ,x1.[entityName]
                            FROM {_initCatalog}.dbo.auditEvent x1
                            join dbo.auditEventType x2 on x2.id = x1.eventType
                            join dbo.audit x3 on x3.id = x1.idAudit 
                            where x1.entityType = 4 and x1.eventType = 300";
            _dataSet = CreateConnetion(connectionString,sql);
        }
        public DataSet GetData()
        {
            logger.Trace($"Выполняю подключение к базе данных : {_dataSource} в каталог {_initCatalog}");
            string connectionString = $"Data source = {_dataSource}; Initial Catalog = {_initCatalog}; Integrated Security=SSPI;";
            string sql = $@"SELECT --x1.[id]
                        --,x1.[idAudit]
                            convert(varchar, x3.timestamp, 104)  timestamp
                        --,x1.[eventType]
                            ,x2.name EventTypeName
                        --,x1.[entityType]
                        ,x1.[entityName]
                            FROM {_initCatalog}.dbo.auditEvent x1
                            join dbo.auditEventType x2 on x2.id = x1.eventType
                            join dbo.audit x3 on x3.id = x1.idAudit 
                            where x1.entityType = 4 and x1.eventType = 300";
            return CreateConnetion(connectionString,sql);
        }

        public string GetCatalog()
        {
            return _initCatalog;
        }
        public DataSet GetDataSet()
        {
            return _dataSet;
        }
       
        private DataSet CreateConnetion(string conString, string sqlCommand)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(conString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand, connection);
                DataSet ds = new DataSet();
                adapter.Fill(ds); 
                return ds;
            }
            }
            catch(Exception ex)
            {
                logger.Error($"Произошла ошибка при обращении к базе данных : {ex.ToString()}");
                return null;
            }
            
        }

    }
}