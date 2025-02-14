using System.Net.Http;
using System.Text;

namespace Fanex.NAP.SPU.DCS.JobService.Services
{
    public static class ApiHelper
    {
        public static async Task<TResponse> PostAsync<TRequest, TResponse>(HttpClient httpClient, string url, TRequest request)
        {
            var response = await httpClient.PostAsync(
                url,
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseContent);
        }

        public static async Task<TResponse> PostAsync<TResponse>(HttpClient httpClient, string url, Dictionary<string, string> queryParams)
        {
            var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result;
            var fullUrl = $"{url}?{queryString}";

            var response = await httpClient.PostAsync(fullUrl, null);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseContent);
        }

        public static async Task<TResponse> GetAsync<TResponse>(HttpClient httpClient, string url)
        {
            var response = await httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseContent);
        }

        public static string GetFullUrl(string baseUrl, string endpoint)
        {
            return $"{baseUrl}{endpoint}";
        }
    }
}
