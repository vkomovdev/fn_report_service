using System.Collections.Generic;
using System.Linq;
using ReportService.Domain;
using Xunit;

namespace ReportBuilderTests
{
    public class ReportBuilderTests
    {
        [Fact]
        public void Build_IncludesAllEmployeesAndTotals()
        {
            var employees = new List<Employee>
            {
                new Employee { Name = "A", Department = "D1", Salary = 100 },
                new Employee { Name = "B", Department = "D1", Salary = 200 },
                new Employee { Name = "C", Department = "D2", Salary = 300 }
            };

            var report = ReportBuilder.Build(2017, 1, employees);

            Assert.Contains("A", report);
            Assert.Contains("B", report);
            Assert.Contains("C", report);
            Assert.Equal(2, report.Split('\n').Count(l => l.Contains("Всего по отделу")));
            Assert.Contains("Всего по предприятию\t600р", report);
        }
    }
}
