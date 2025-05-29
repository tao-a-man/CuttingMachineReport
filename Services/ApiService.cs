using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CuttingMachineReport.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetPcCuttingConnectString(string jsonData)
        {
            var requestUri = $"{baseUrl}/external/pc_cutting_get_connect_string";
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(requestUri, content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                throw new Exception("Error calling API: " + e.Message, e);
            }
        }

        public async Task<string> PostPcCuttingReportAsync(string jsonData)
        {
            var requestUri = $"{baseUrl}/external/pc_cutting_report";
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(requestUri, content);
                response.EnsureSuccessStatusCode(); 

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody; 
            }
            catch (HttpRequestException e)
            {
                throw new Exception("Error calling API: " + e.Message, e);
            }
        }
        public async Task<string> PostPcCuttingInfo(string jsonData)
        {
            var requestUri = $"{baseUrl}/external/pc_cutting_report_get_config";
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(requestUri, content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                throw new Exception("Error calling API: " + e.Message, e);
            }
        }
    }
}
