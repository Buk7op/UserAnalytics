
using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using OfficeOpenXml;
using System.Data;
using NLog;

namespace UserAnalytics
{
    class DashboardCreator
    {
        
        public static void Create(DataSet ds,Dictionary<string,string> UniqueUsers,string folderName)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Trace("Создаю Excel-файл");
            string filePath = ConfigurationManager.AppSettings.Get("childDir1") + @"\Dashboard\Dashboard.xlsm";
            FileInfo file = new FileInfo(filePath);
            using (ExcelPackage excelPackage = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[2];
                int end = worksheet.Cells.Where(c => c.Start.Column == 2 && !c.Value.ToString().Equals("")).Last().End.Row + 1;              
                foreach(DataRow item in ds.Tables[0].Rows)
                {
                    worksheet.Cells[end,1].Value = item.ItemArray[2].ToString(); 
                    worksheet.Cells[end,2].Value = item.ItemArray[0].ToString();
                    worksheet.Cells[end,3].Value = folderName;
                    end++;
                }

                end = worksheet.Cells.Where(c => c.Start.Column == 5 && !c.Value.ToString().Equals("")).Last().End.Row + 1;
                
                foreach (var item in UniqueUsers.Keys)
                {
                    worksheet.Cells[end,5].Value = item;
                    worksheet.Cells[end,6].Value = UniqueUsers[item];
                    worksheet.Cells[end,7].Value = folderName;
                    end++;
                }
                
                  
                excelPackage.SaveAs(file);
            }
            
        }
        public static void ClearTable()
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Trace("Очищаю таблицу");
            string filePath = ConfigurationManager.AppSettings.Get("childDir1") + @"\Dashboard\Dashboard.xlsm";
            FileInfo file = new FileInfo(filePath);
            using (ExcelPackage excelPackage = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[2];
                int end = worksheet.Cells.Where(c => c.Start.Column == 2 && !c.Value.ToString().Equals("")).Last().End.Row;
                if(!Convert.ToBoolean(ConfigurationManager.AppSettings.Get("Parent")))
                {
                    for(int i = 2; i <= end; i++)
                    {
                        worksheet.Cells[i,1].Clear();
                        worksheet.Cells[i,2].Clear();
                        worksheet.Cells[i,3].Clear();
                    }
                }
                excelPackage.SaveAs(file);
            }
            
        }
    }
}