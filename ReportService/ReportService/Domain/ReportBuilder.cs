using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportService.Domain
{
    public static class ReportBuilder
    {
        public static string Build(int year, int month, IEnumerable<Employee> employees)
        {
            var sb = new StringBuilder();
            sb.AppendLine(MonthNameResolver.MonthName.GetName(year, month));
            sb.AppendLine();

            int companyTotal = 0;
            foreach (var dept in employees.GroupBy(e => e.Department))
            {
                sb.AppendLine("--------------------------------------------");
                sb.AppendLine(dept.Key);
                foreach (var emp in dept)
                {
                    sb.AppendLine($"{emp.Name}\t{emp.Salary}р");
                }
                var depTotal = dept.Sum(e => e.Salary);
                sb.AppendLine($"Всего по отделу\t{depTotal}р");
                sb.AppendLine();
                companyTotal += depTotal;
            }

            sb.AppendLine("--------------------------------------------");
            sb.AppendLine($"Всего по предприятию\t{companyTotal}р");
            return sb.ToString();
        }
    }
}
