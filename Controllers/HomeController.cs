using HealthifyConsume.Areas.Admin.Models;
using HealthifyConsume.BAL;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HealthifyConsume.Controllers
{
    [CheckAccess]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

		public IActionResult Profile()
		{
			if (HttpContext.Session.GetInt32("UserID") != null)
			{
				string firstName = HttpContext.Session.GetString("FirstName");
				string lastName = HttpContext.Session.GetString("LastName");
				string email = HttpContext.Session.GetString("Email");
				string username = HttpContext.Session.GetString("UserName");
				ViewData["firstName"] = firstName;
				ViewData["lastName"] = lastName;
				ViewData["email"] = email;
				ViewData["username"] = username;



				return View("Profile");
			}
			else
			{
				// Redirect to login if user is not logged in
				return RedirectToAction("Login");
			}
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}