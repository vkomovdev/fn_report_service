using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ReportService.Domain
{
    public interface ISalaryProvider
    {
        Task<int> GetSalaryAsync(string buhCode, CancellationToken ct = default);
    }

    public class SalaryProvider : ISalaryProvider
    {
        private readonly HttpClient _http;
        public SalaryProvider(HttpClient http) => _http = http;

        public async Task<int> GetSalaryAsync(string code, CancellationToken ct = default)
        {
            var resp = await _http.GetAsync($"/api/empcode/{code}", ct);
            resp.EnsureSuccessStatusCode();
            var s = await resp.Content.ReadAsStringAsync(ct);
            return (int)decimal.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
