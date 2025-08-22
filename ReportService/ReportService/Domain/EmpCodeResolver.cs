using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ReportService.Domain
{
    public interface IEmpCodeResolver
    {
        Task<string> GetCodeAsync(string inn, CancellationToken ct = default);
    }
    public class EmpCodeResolver: IEmpCodeResolver
    {
        private readonly HttpClient _http;
        public EmpCodeResolver(HttpClient http) => _http = http;

        public async Task<string> GetCodeAsync(string inn, CancellationToken ct = default)
        {
            var resp = await _http.GetAsync($"/api/inn/{inn}", ct);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync(ct);
        }
    }
}
