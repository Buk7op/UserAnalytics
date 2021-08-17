using System;
using System.Data;
using System.Collections.Generic;

namespace UserAnalytics
{
    class Sorter
    {
        public Dictionary<string,string> FindUnique(DataSet dataSet)
        {
            Dictionary <string, string> uniqueUsers = new Dictionary<string, string>();
            DateTime month = new DateTime(DateTime.Now.Year,DateTime.Now.Month,01);
            foreach (DataRow item in dataSet.Tables[0].Rows)
            {
                DateTime time = DateTime.Parse(item.ItemArray[0].ToString());
                if (!uniqueUsers.ContainsKey(item.ItemArray[2].ToString().ToLower()) && DateTime.Now > time && time >= month)
                {
                    uniqueUsers.Add(item.ItemArray[2].ToString().ToLower(),item.ItemArray[0].ToString());
                }
            }
            
            return uniqueUsers;
        }

        public List<string> LastMonthUsers(DataSet dataSet)
        {
            List <string> monthUsers = new List<string>();
            DateTime month = new DateTime(DateTime.Now.Year,DateTime.Now.Month,01);
            foreach (DataRow item in dataSet.Tables[0].Rows)
            {
                DateTime time = DateTime.Parse(item.ItemArray[0].ToString());
                if (DateTime.Now > time && time >= month)
                {
                    monthUsers.Add($"{item.ItemArray[2].ToString().ToLower()},{item.ItemArray[0].ToString()}");
                }
            }
            return monthUsers;
        }
    }
}