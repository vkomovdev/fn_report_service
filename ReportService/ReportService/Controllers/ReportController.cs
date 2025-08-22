using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using ReportService.Domain;

namespace ReportService.Controllers
{
    [Route("api/[controller]")]
    public class ReportController: Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmpCodeResolver _codeResolver;
        private readonly ISalaryProvider _salaryProvider;

        public ReportController(
            IEmployeeRepository employeeRepository,
            IEmpCodeResolver codeResolver,
            ISalaryProvider salaryProvider)
        {
            _employeeRepository = employeeRepository;
            _codeResolver = codeResolver;
            _salaryProvider = salaryProvider;
        }

        [HttpGet]
        [Route("{year}/{month}")]
        public async Task<IActionResult> Download(int year, int month)
        {
            var employees = await _employeeRepository.GetEmployeesAsync(HttpContext.RequestAborted);

            await Parallel.ForEachAsync(
                employees,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = Math.Min(16, Environment.ProcessorCount * 2),
                    CancellationToken = HttpContext.RequestAborted
                },
                async (emp, ct) =>
                {
                    emp.BuhCode = await _codeResolver.GetCodeAsync(emp.Inn, ct);
                    emp.Salary = await _salaryProvider.GetSalaryAsync(emp.BuhCode, ct);
                }
            );

            //employees = new List<Employee>
            //{
            //    new Employee { Name = "A", Department = "D1", Salary = 100, BuhCode = "1", Inn = "2" },
            //    new Employee { Name = "B", Department = "D1", Salary = 200, BuhCode = "1", Inn = "2" },
            //    new Employee { Name = "C", Department = "D2", Salary = 300, BuhCode = "1", Inn = "2" }
            //};

            var reportText = ReportBuilder.Build(year, month, employees);
            var bytes = Encoding.UTF8.GetBytes(reportText);
            return File(bytes, "application/octet-stream", "report.txt");
        }
    }
}
