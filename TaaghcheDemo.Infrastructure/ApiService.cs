using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TaaghcheDemo.Infrastructure
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
        public async Task<string> GetJsonDataAsString(string apiUrl, int bookId)
        {

            if (string.IsNullOrEmpty(apiUrl))
                throw new ArgumentException("API URL cannot be null or empty.", nameof(apiUrl));
            try
            {
                string fullUrl = apiUrl + "/" + bookId;
                HttpResponseMessage response = await _httpClient.GetAsync(fullUrl);
                response.EnsureSuccessStatusCode();
                string jsonData = await response.Content.ReadAsStringAsync();
                return jsonData;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
        }
    }
}
