using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ReportService.Domain
{
    public interface ISalaryProvider
    {
        Task<int> GetSalaryAsync(string buhCode);
    }

    public class SalaryProvider : ISalaryProvider
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public SalaryProvider(HttpClient client, string baseUrl)
        {
            _client = client;
            _baseUrl = baseUrl?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(baseUrl));
        }

        public async Task<int> GetSalaryAsync(string buhCode)
        {
            var response = await _client.GetStringAsync($"{_baseUrl}/api/empcode/{buhCode}");
            return (int)decimal.Parse(response);
        }
    }
}
