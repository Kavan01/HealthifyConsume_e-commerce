using HealthifyConsume.Areas.Admin.Models;
using HealthifyConsume.BAL;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace HealthifyConsume.Areas.Admin.Controllers
{
    [CheckAccess]
    [Area("Admin")]
	[Route("Admin/{Controller}/{Action}")]
	public class DashboardController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5176/api");
        private readonly HttpClient _client;

        public DashboardController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GET()
        {
            List<DashboardModel> dash = new List<DashboardModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Dashboard/Get").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonobject = JsonConvert.DeserializeObject(data);

                var dataofObject = jsonobject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataofObject, Formatting.Indented);
                dash = JsonConvert.DeserializeObject<List<DashboardModel>>(extractedDataJson);
                

			}

            return View("DashboardList", dash);
        }
    }
}