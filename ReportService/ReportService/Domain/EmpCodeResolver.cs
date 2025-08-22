using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ReportService.Domain
{
    public interface IEmpCodeResolver
    {
        Task<string> GetCodeAsync(string inn);
    }
    public class EmpCodeResolver: IEmpCodeResolver
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public EmpCodeResolver(HttpClient client, string baseUrl)
        {
            _client = client;
            _baseUrl = baseUrl?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(baseUrl));
        }
        public Task<string> GetCodeAsync(string inn)
        {
            return _client.GetStringAsync($"{_baseUrl}/api/inn/{inn}");
        }
    }
}
