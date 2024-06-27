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
	public class CustomerController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5176/api");
        private readonly HttpClient _client;

        public CustomerController()
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
            List<CustomerModel> custs = new List<CustomerModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Customer/Get").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonobject = JsonConvert.DeserializeObject(data);

                var dataofObject = jsonobject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataofObject, Formatting.Indented);
                custs = JsonConvert.DeserializeObject<List<CustomerModel>>(extractedDataJson);
            }

            return View("CustomerList", custs);
        }

        [HttpGet]
        public IActionResult Delete(int CustomerID)
        {
            HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/Customer/Delete/?CustomerID=" + CustomerID).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Customer Deleted";
            }
            return RedirectToAction("GET");
        }

        [HttpGet]
        public IActionResult Edit(int CustomerID)
        {
            CustomerModel custs = new CustomerModel();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Customer/Get/{CustomerID}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonobject = JsonConvert.DeserializeObject(data);

                var dataofObject = jsonobject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataofObject, Formatting.Indented);
                custs = JsonConvert.DeserializeObject<CustomerModel>(extractedDataJson);
            }

            return View("CustomerAddEdit", custs);
        }

        [HttpPost]
        public async Task<IActionResult> Save(CustomerModel modelCust)
        {

            try
            {
                MultipartFormDataContent formData = new MultipartFormDataContent();
                formData.Add(new StringContent(modelCust.CustomerName), "CustomerName");
                formData.Add(new StringContent(modelCust.Email), "Email");
                formData.Add(new StringContent(modelCust.Password), "Password");
                formData.Add(new StringContent(modelCust.Address), "Address");
                formData.Add(new StringContent(modelCust.Contact), "Contact");

                if (modelCust.CustomerID == null)
                {
                    HttpResponseMessage response = await _client.PostAsync($"{_client.BaseAddress}/Customer/Insert", formData);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "Customer Inserted";
                        return RedirectToAction("GET");
                    }
                }
                else
                {
                    HttpResponseMessage response = await _client.PutAsync($"{_client.BaseAddress}/v/Update/?CustomerID={modelCust.CustomerID}", formData);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "Customer Updated";
                        return RedirectToAction("GET");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error Occurred" + ex.Message;
            }
            return RedirectToAction("GET");
        }


    }
}
