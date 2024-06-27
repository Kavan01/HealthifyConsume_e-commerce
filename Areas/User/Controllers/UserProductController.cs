using HealthifyConsume.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HealthifyConsume.Areas.User.Controllers
{
    public class UserProductController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5176/api");
        private readonly HttpClient _client;

        public UserProductController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        public IActionResult Index()
        {
            
            return View();
        }
        
    }
}
