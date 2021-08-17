using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using NLog;
using OfficeOpenXml;
using System.Linq;
namespace UserAnalytics 
{
    class DataWriter
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        Dictionary<string,string> filePath = new Dictionary<string, string>();
        

        public Dictionary<string,string> GetFilePath() 
        {
            return filePath;
        }
        public void ClearFilePath() 
        {
            filePath.Clear();
        }

        private string CreateDir(string folderName,string name)
        {
            string configPath = ConfigurationManager.AppSettings.Get("dir");
            string path =$@"{configPath}\{folderName}" ;
            string subpath = $@"{DateTime.Now.ToShortDateString()}";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            dirInfo.CreateSubdirectory(subpath);
            if(name != "UserList.csv")
            filePath.Add($@"{dirInfo.FullName}\{subpath}\{name}",name);
            return $@"{dirInfo.FullName}\{subpath}";
        }

        public void WriteUserList (DataSet ds,string folderName)
        {
            string name = "UserList.csv";
            logger.Trace($"Создаю список пользователей UserList.csv в {folderName}");
            StringBuilder str = new StringBuilder();
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                str.Append(item.ItemArray[0].ToString()+",");
                str.Append(item.ItemArray[2].ToString()+Environment.NewLine);
                
            }
            try
            {
                File.AppendAllText($@"{CreateDir(folderName,name)}\{name}", str.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                logger.Error($"Ошибка при записи данных в csv файл : {ex.ToString()}");
            }
        }
        public void WriteUniqueUserList (Dictionary<string, string> dict,string folderName) 
        {
            string name = "UniqueUserList.xlsx";
            logger.Trace($"Создаю список уникальных пользователей UniqueUserList.xlsx в {folderName}");
            DateTime month = new DateTime(DateTime.Now.Year,DateTime.Now.Month,01);
            
            if(dict.Count == 0)
            {
                dict.Add("Нет уникальных пользователей",$" начиная с {month}");
            }
            
            try
            {
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    excelPackage.Workbook.Properties.Title = "UniqueUserList";
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Уникальные пользователи");
                    int i = 1;
                    foreach (var item in dict)
                    {
                        worksheet.Cells[i,1].Value = item.Key;
                        worksheet.Cells[i,2].Value = item.Value;
                        i++;
                    }
                    FileInfo fi = new FileInfo($@"{CreateDir(folderName,name)}\{name}");
                    excelPackage.SaveAs(fi);
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Ошибка при записи данных в xlsx файл : {ex.ToString()}");
            }   
        }
        public void WriteLastMonthUserList (List<string> list,string folderName) 
        {
            string name = "LastMonthUserList.xlsx";
            logger.Trace($"Создаю список пользователей за месяц LastMonthUserList.xlsx в {folderName}");
            DateTime month = new DateTime(DateTime.Now.Year,DateTime.Now.Month,01);
            if(list.Count == 0)
            {
                list.Add($"Нет пользователей, начиная с {month}");
            }
            
            try
            {
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    excelPackage.Workbook.Properties.Title = "LastMonthUserList";
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Пользователей за месяц");
                    int i = 1;
                    foreach (var item in list)
                    {
                        string [] s = item.Split(",");
                        worksheet.Cells[i,1].Value = s[0];
                        worksheet.Cells[i,2].Value = s[1];
                        i++;
                    }
                    FileInfo fi = new FileInfo($@"{CreateDir(folderName,name)}\{name}");
                    excelPackage.SaveAs(fi);
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Ошибка при записи данных в xlsx файл : {ex.ToString()}");
            }
        }

        public void Summary(Dictionary<string,string> UniqueUsers, List<string> monthUsers, string folderName)
        {
            string name = "Summary.txt";
            logger.Trace($"Создаю итог Summary.txt в {folderName}");
            DateTime month = new DateTime(DateTime.Now.Year,DateTime.Now.Month,01);
            StringBuilder str = new StringBuilder();
            str.Append($"Начиная с {month} и до {DateTime.Now} на {folderName} было зафиксировано:"+Environment.NewLine);
            str.Append($"{UniqueUsers.Count} уникальных пользователей"+Environment.NewLine);
            str.Append($"{monthUsers.Count} входов"+Environment.NewLine);
            try
            {
                File.AppendAllText($@"{CreateDir(folderName,name)}\{name}", str.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                logger.Error($"Ошибка при записи данных в csv файл : {ex.ToString()}");
            }
        }

        public void CreateExcel(DataSet ds,Dictionary<string,string> UniqueUsers,string folderName)
        {
            logger.Trace("Создаю дашборд");
            string path = ConfigurationManager.AppSettings.Get("childDir1") + @"\Dashboard\Dashboard.xlsm";
            DashboardCreator.Create(ds,UniqueUsers,folderName);
            filePath.Add(path,"Dashboard.xlsm");
        }

        
    }
}