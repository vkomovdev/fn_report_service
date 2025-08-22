using System.Globalization;
using System;

namespace ReportService.Domain
{
    public static class MonthName
    {
        public static string GetName(int year, int monthNum)
        {
            var date = new DateTime(year, monthNum, 1).ToString("MMMMMM yyyy", CultureInfo.GetCultureInfo("ru-RU"));
            return CultureInfo.GetCultureInfo("ru-RU").TextInfo.ToTitleCase(date);
        }
    }
}
