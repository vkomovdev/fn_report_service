using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReportService.Controllers;
using ReportService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReportBuilderTests
{
    public class ReportControllerTests
    {
        private class FakeEmployeeRepository : IEmployeeRepository
        {
            public Task<List<Employee>> GetEmployeesAsync(CancellationToken ct = default)
            {
                return Task.FromResult(new List<Employee>
                {
                    new Employee { Name = "Андрей Сергеевич Бубнов", Department = "ФинОтдел", Inn = "1" },
                    new Employee { Name = "Василий Васильевич Кузнецов", Department = "Бухгалтерия", Inn = "2" }
                });
            }
        }
        private class FakeEmpCodeResolver : IEmpCodeResolver
        {
            public Task<string> GetCodeAsync(string inn, CancellationToken ct = default)
            {
                return Task.FromResult(inn switch
                {
                    "1" => "BC",
                    "2" => "AD",
                    _ => "UNKNOWN"
                });
            }
        }

        private class FakeSalaryProvider : ISalaryProvider
        {
            public Task<int> GetSalaryAsync(string buhCode, CancellationToken ct = default)
            {
                return Task.FromResult(buhCode switch
                {
                    "BC" => 70000,
                    "AD" => 50000,
                    _ => 0
                });
            }
        }

        [Fact]
        public async Task Download_ReturnsReportFile()
        {
            var controller = new ReportController(
                new FakeEmployeeRepository(),
                new FakeEmpCodeResolver(),
                new FakeSalaryProvider());

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await controller.Download(2024, 5);

            var file = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/octet-stream", file.ContentType);
            Assert.Equal("report.txt", file.FileDownloadName);
            var text = Encoding.UTF8.GetString(file.FileContents);
            Assert.Contains("Всего по предприятию", text);
            Assert.Contains("Май 2024", text);
            Assert.Contains("ФинОтдел", text);
            Assert.Contains("Бухгалтерия", text);
            Assert.Contains("Андрей Сергеевич Бубнов 70000р", text);
            Assert.Contains("Василий Васильевич Кузнецов 50000р", text);
        }
    }
}
