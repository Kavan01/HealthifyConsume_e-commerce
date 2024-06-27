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
    public class CategoryController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5176/api");
        private readonly HttpClient _client;

        public CategoryController()
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
            List<CategoryModel> cats = new List<CategoryModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Category/Get").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonobject = JsonConvert.DeserializeObject(data);

                var dataofObject = jsonobject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataofObject, Formatting.Indented);
                cats = JsonConvert.DeserializeObject<List<CategoryModel>>(extractedDataJson);
            }

            return View("CategoryList", cats);
        }

        [HttpGet]
        public IActionResult Delete(int CategoryID)
        {
            HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/Category/Delete/?CategoryID=" + CategoryID).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Category Deleted";
            }
            return RedirectToAction("GET");
        }

        [HttpGet]
        public IActionResult Edit(int CategoryID)
        {
            CategoryModel cats = new CategoryModel();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Category/Get/{CategoryID}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonobject = JsonConvert.DeserializeObject(data);

                var dataofObject = jsonobject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataofObject, Formatting.Indented);
                cats = JsonConvert.DeserializeObject<CategoryModel>(extractedDataJson);
            }

            return View("CategoryAddEdit", cats);
        }

        [HttpPost]
        public async Task<IActionResult> Save(CategoryModel modelCat)
        {

            try
            {
                MultipartFormDataContent formData = new MultipartFormDataContent();
                formData.Add(new StringContent(modelCat.CategoryName), "CategoryName");

                if (modelCat.CategoryID == null)
                {
                    HttpResponseMessage response = await _client.PostAsync($"{_client.BaseAddress}/Category/Insert", formData);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "Category Inserted";
                        return RedirectToAction("GET");
                    }
                }
                else
                {
                    HttpResponseMessage response = await _client.PutAsync($"{_client.BaseAddress}/Category/Update/?CategoryID={modelCat.CategoryID}", formData);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "Category Updated";
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
