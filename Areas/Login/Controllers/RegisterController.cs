using HealthifyConsume.Areas.Login.DAL;
using HealthifyConsume.Areas.Admin.Models;
using HealthifyConsume.Areas.Login.Models;
using HealthifyConsume.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthifyConsume.Areas.Login.Controllers
{
    [Area("Login")]
	[Route("Login/{Controller}/{Action}")]
	public class RegisterController : Controller
    {
        Uri baseuri = new Uri("http://localhost:5176/api");
        DAL_Helper dal = new DAL_Helper();
        private readonly HttpClient _Client;
        public RegisterController()
        {
            _Client = new HttpClient();
            _Client.BaseAddress = baseuri;
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Save(RegistrationModel registration)
        {
            try
            {
                MultipartFormDataContent formData = new MultipartFormDataContent();
                formData.Add(new StringContent(registration.FirstName), "FirstName");
                formData.Add(new StringContent(registration.LastName), "LastName");
                formData.Add(new StringContent(registration.UserName), "UserName");
                formData.Add(new StringContent(registration.Password), "Password");
                formData.Add(new StringContent(registration.Email), "Email");
                formData.Add(new StringContent(Convert.ToString(registration.IsActive)), "IsActive");
                formData.Add(new StringContent(Convert.ToString(registration.IsAdmin)), "IsAdmin");

                HttpResponseMessage response = await _Client.PostAsync($"{_Client.BaseAddress}/Register/Insert", formData);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Person Inserted";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An Error Occured" + ex.Message;
            }
            return RedirectToAction("Login");
        }


        public async Task<IActionResult> LoginCheck(LoginModel model)
        {
            if (string.IsNullOrEmpty(model.UserName))
            {
                ModelState.AddModelError("UserName", "Enter Username");
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("Password", "Enter Password");
            }
            string apiUrl = $"/Login/Get/{model.UserName}/{model.Password}";
            Console.WriteLine(apiUrl);
            if (ModelState.IsValid)
            {
                var apiResponse = await dal.SendRequestAsync<RegistrationModel>(apiUrl, HttpMethod.Get);


                if (apiResponse.IsSuccess)
                {
                    RegistrationModel user = apiResponse.Data;
                    Console.WriteLine(user.IsActive);
                    HttpContext.Session.SetInt32("UserID", user.UserID);
                    HttpContext.Session.SetString("FirstName", user.FirstName);
                    HttpContext.Session.SetString("LastName", user.LastName);
                    HttpContext.Session.SetString("UserName", user.UserName);
                    HttpContext.Session.SetString("Password", user.Password);
                    HttpContext.Session.SetString("Email", user.Email);

                    HttpContext.Session.SetInt32("IsActive", Convert.ToInt32(user.IsActive));
                    HttpContext.Session.SetInt32("IsAdmin", Convert.ToInt32(user.IsAdmin));

                    if (HttpContext.Session.GetInt32("UserID") != null && HttpContext.Session.GetString("UserName") != null && HttpContext.Session.GetInt32("IsActive") != null)
                    {
                        if (HttpContext.Session.GetInt32("IsActive") == 1)
                        {
                            return RedirectToAction("GET", "Dashboard", new { area = "Admin" });
                        }
                        else
                        {
                            return RedirectToAction("GET", "MainPage", new { area = "User" });
                        }
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = "Invalid Username or Password";

                    }

                    /* return RedirectToAction("GET", "Dashboard", new {area ="Admin"});*/

                }
                else
                {
                    @ViewData["ErrorMessage"] = "Invalid Username or Password";
                }
            }

            return View("Login", model);
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



		public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Login");
        }

    }
}
