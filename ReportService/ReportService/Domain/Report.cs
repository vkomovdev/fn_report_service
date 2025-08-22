using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportService.Domain
{
    [Obsolete]
    public class Report
    {
        public string S { get; set; }
        public void Save()
        {
            System.IO.File.WriteAllText("D:\\report.txt", S);
        }
    }
}
