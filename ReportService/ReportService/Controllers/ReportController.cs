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
        private readonly string _connString;
        private readonly IEmpCodeResolver _codeResolver;
        private readonly ISalaryProvider _salaryProvider;

        public ReportController(IConfiguration configuration)
        {
            _connString = configuration.GetConnectionString("EmployeeDb");
            _codeResolver = new EmpCodeResolver(new HttpClient(), configuration["EmpCodeService:BaseUrl"]);
            _salaryProvider = new SalaryProvider(new HttpClient(), configuration["SalaryService:BaseUrl"]);
        }

        [HttpGet]
        [Route("{year}/{month}")]
        public async Task<IActionResult> Download(int year, int month)
        {
            var employees = new List<Employee>();

            using (var conn = new NpgsqlConnection(_connString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT e.name, e.inn, d.name FROM emps e JOIN deps d ON e.departmentid = d.id WHERE d.active = true", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employees.Add(new Employee
                        {
                            Name = reader.GetString(0),
                            Inn = reader.GetString(1),
                            Department = reader.GetString(2)
                        });
                    }
                }
            }

            foreach (var emp in employees)
            {
                emp.BuhCode = await _codeResolver.GetCodeAsync(emp.Inn);
                emp.Salary = await _salaryProvider.GetSalaryAsync(emp.BuhCode);
            }

            var reportText = ReportBuilder.Build(year, month, employees);
            var bytes = Encoding.UTF8.GetBytes(reportText);
            return File(bytes, "application/octet-stream", "report.txt");
        }
    }
}
