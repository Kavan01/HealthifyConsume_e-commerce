using HealthifyConsume.Areas.Admin.Models;
using HealthifyConsume.Areas.User.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using System.Text.Json.Nodes;

namespace HealthifyConsume.Areas.User.Controllers
{
    [Area("User")]
    [Route("User/{Controller}/{Action}")]
    public class MainPageController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5176/api");
        private readonly HttpClient _client;

        public MainPageController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

       
        public async Task<IActionResult> GET(string productList)
        {
            if(productList == null)
            {
                using (var client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    CommonModel commons = new CommonModel();
                    HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Product/Get").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        commons.productListResponse = JsonConvert.DeserializeObject<ProductListResponse>(data);
                        commons.categoryListResponse = await GetCategory();
                        return View("MainPage", commons);
                    }
                    else
                    {
                        Console.WriteLine("Error in api");
                    }

                }
            }
            else
            {
                CommonModel commons = new CommonModel();
                commons.productListResponse = JsonConvert.DeserializeObject<ProductListResponse>(productList);
                commons.categoryListResponse = await GetCategory();
                return View("MainPage", commons);
            }
            return View("MainPage");
            

        }


        public async Task<CategoryListResponse> GetCategory()
        {
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                CategoryListResponse category = new CategoryListResponse();
                HttpResponseMessage res  = _client.GetAsync($"{_client.BaseAddress}/Category/Get").Result;                
                if (res.IsSuccessStatusCode)
                {
                    string data = await res.Content.ReadAsStringAsync();
                    category = JsonConvert.DeserializeObject<CategoryListResponse>(data);

                    return category;
                }
                return category;
            }

        }
        
        public async Task<IActionResult> GetProductByCategory(int id)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                CommonModel common = new CommonModel();

                HttpResponseMessage res = _client.GetAsync($"{_client.BaseAddress}/Product/GetProductByCategory?CategoryID=" + id).Result;
                if (res.IsSuccessStatusCode)
                {
                    string data = res.Content.ReadAsStringAsync().Result;
                    common.productListResponse = JsonConvert.DeserializeObject<ProductListResponse>(data);
                    return RedirectToAction("GET", new { productList = JsonConvert.SerializeObject(common.productListResponse) });
                }

                else
                {
                    Console.WriteLine("Error in api");
                    return RedirectToAction("GET");
                }
                return View("MainPage");
            }
               
        }
    }
}
