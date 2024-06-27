using Newtonsoft.Json;
using System.Net;

namespace HealthifyConsume.Areas.Login.DAL
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class DAL_Helper
    {
        #region HttpClient
        private readonly HttpClient _httpClient;
        private Uri BaseURL = new Uri("http://localhost:5176");


        public DAL_Helper()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = BaseURL;
        }
        #endregion



        public async Task<ApiResponse<T>> SendRequestAsync<T>(string url, HttpMethod method, HttpContent content = null)
        {



            var response = await _httpClient.SendAsync(new HttpRequestMessage(method, $"/api{url}") { Content = content });

            var apiResponse = new ApiResponse<T>
            {
                StatusCode = response.StatusCode,
                IsSuccess = response.IsSuccessStatusCode
            };

            if (apiResponse.IsSuccess)
            {
                var responseBody = response.Content.ReadAsStringAsync().Result;
                dynamic apiRes = JsonConvert.DeserializeObject(responseBody);
                var DataJson = apiRes.data;
                var apiExtratedDataJson = JsonConvert.SerializeObject(DataJson, Formatting.Indented);
                apiResponse.Data = JsonConvert.DeserializeObject<T>(apiExtratedDataJson);
                apiResponse.Message = apiRes.msg;
            }
            else
            {
                apiResponse.ErrorMessage = "Request failed.";
            }
            return apiResponse;
        }

    }
}
