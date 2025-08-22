using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace ReportService.Domain
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetEmployeesAsync(CancellationToken ct = default);
    }

    public class EmployeeRepository: IEmployeeRepository
    {
        private readonly string _connString;
        public EmployeeRepository(string connString)
        {
            _connString = connString;
        }

        public async Task<List<Employee>> GetEmployeesAsync(CancellationToken ct = default)
        {
            var employees = new List<Employee>();
            await using var conn = new NpgsqlConnection(_connString);
            await conn.OpenAsync(ct);
            const string sql = "SELECT e.name, e.inn, d.name FROM emps e JOIN deps d ON e.departmentid = d.id WHERE d.active = true";
            await using var cmd = new NpgsqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                employees.Add(new Employee
                {
                    Name = reader.GetString(0),
                    Inn = reader.GetString(1),
                    Department = reader.GetString(2)
                });
            }
            return employees;
        }
    }
}
