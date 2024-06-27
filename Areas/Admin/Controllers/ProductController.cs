using ClosedXML.Excel;
using HealthifyConsume.Areas.Admin.Models;
using HealthifyConsume.BAL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Drawing;
using System.Net.Http;
using System.Xml.Linq;
using Font = iTextSharp.text.Font;

namespace HealthifyConsume.Areas.Admin.Controllers
{
    [CheckAccess]
    [Area("Admin")]
	[Route("Admin/{Controller}/{Action}")]
	public class ProductController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5176/api");
        private readonly HttpClient _client;

        public ProductController()
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
            List<ProductModel> products = new List<ProductModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Product/Get").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonobject = JsonConvert.DeserializeObject(data);

                var dataofObject = jsonobject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataofObject, Formatting.Indented);
                products = JsonConvert.DeserializeObject<List<ProductModel>>(extractedDataJson);
            }

            return View("ProductList", products);
        }

        public DataTable ProductList()
        {

            DataTable dataTable = new DataTable();

            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Product/Get").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                var dataOfObject = jsonObject.data;
                var extractdDatajson = JsonConvert.SerializeObject(dataOfObject, Formatting.Indented);
                dataTable = JsonConvert.DeserializeObject<DataTable>(extractdDatajson);
            }

            return dataTable;
        }

        public FileResult Export_Product_List_To_Excel()
        {
            DataTable dataTable = ProductList();

            using (XLWorkbook wb = new XLWorkbook())
            {
                IXLWorksheet ws = wb.Worksheets.Add("Product Statements");

                // Adding the DataTable data to the worksheet starting from cell A1
                var tableRange = ws.Cell(1, 1).InsertTable(dataTable, true).AsRange();

                // Adjust column widths to fit contents
                ws.Columns().AdjustToContents();

                // Apply styling to header row
                var headerRow = tableRange.FirstRow();
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Set the row height and cell alignment for data rows
                foreach (var row in tableRange.RowsUsed().Skip(1))
                {
                    ws.Row(row.RowNumber()).Height = 20; // Set the height for the row
                    foreach (var cell in row.CellsUsed())
                    {
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    string fileName = "Product_Lists.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }



        public FileResult Export_Product_List_To_pdf()
        {
            DataTable dataTable = ProductList();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Custom page size
                iTextSharp.text.Rectangle customPageSize = new iTextSharp.text.Rectangle(2300, 1200);
                using (Document document = new Document(customPageSize))
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    // Define fonts
                    BaseFont boldBaseFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.WINANSI, BaseFont.EMBEDDED);

                    Font boldFont = new Font(boldBaseFont, 12);


                    // Title
                    Paragraph title = new Paragraph("Product List", new Font(boldBaseFont, 35));
                    title.Alignment = Element.ALIGN_CENTER;
                    document.Add(title);
                    document.Add(new Chunk("\n"));


                    // Table setup
                    PdfPTable pdfTable = new PdfPTable(dataTable.Columns.Count)
                    {
                        WidthPercentage = 100,
                        DefaultCell = { Padding = 10 }
                    };

                    // Headers
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        Font headerFont = boldFont;
                        PdfPCell headerCell = new PdfPCell(new Phrase(column.ColumnName, headerFont))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 10
                        };
                        pdfTable.AddCell(headerCell);
                    }

                    // Data rows
                    foreach (DataRow row in dataTable.Rows)
                    {
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            var item = row[column];
                            Font itemFont = boldFont;

                            PdfPCell dataCell = new PdfPCell(new Phrase(item?.ToString(), itemFont))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                Padding = 10
                            };
                            pdfTable.AddCell(dataCell);
                        }
                    }

                    document.Add(pdfTable);
                    document.Close();
                }

                // File result
                string fileName = "Product.pdf";
                return File(memoryStream.ToArray(), "application/pdf", fileName);
            }


        }
        /*[HttpGet]
        public IActionResult Delete(int ProductID)
        {
            HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/Product/Delete/?ProductID=" + ProductID).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Product Deleted";
            }
            return RedirectToAction("GET");
        }*/

        [HttpGet]
        public IActionResult Delete(string[] ProductIDlist)
        {
            // Check if EmpIDlist is not null and contains IDs
            if (ProductIDlist != null && ProductIDlist.Length > 0)
            {
                // Convert the array of IDs to a comma-separated string
                string idList = string.Join(",", ProductIDlist);

                // Call the API to delete the multiple employees
                HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/Product/Delete/?ProductIDlist=" + idList).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Product Deleted Successfully";
                }
                else
                {
                    TempData["Message1"] = "Failed to Delete TaskAssign";
                }
            }
            else
            {
                TempData["Message1"] = "No TaskAssign Selected for Deletion";
            }

            return RedirectToAction("GET");
        }

        [HttpGet]
        public IActionResult Edit(int ProductID)
        {
            List<CategoryDropdownModel> cats = new List<CategoryDropdownModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Product/GetCategory").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonobject = JsonConvert.DeserializeObject(data);

                var dataofObject = jsonobject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataofObject, Formatting.Indented);
                cats = JsonConvert.DeserializeObject<List<CategoryDropdownModel>>(extractedDataJson);
            }
            ViewBag.Category = cats;

            ProductModel products = new ProductModel();
            HttpResponseMessage response1 = _client.GetAsync($"{_client.BaseAddress}/Product/Get/{ProductID}").Result;
            if (response1.IsSuccessStatusCode)
            {
                string data = response1.Content.ReadAsStringAsync().Result;
                dynamic jsonobject = JsonConvert.DeserializeObject(data);

                var dataofObject = jsonobject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataofObject, Formatting.Indented);
                products = JsonConvert.DeserializeObject<ProductModel>(extractedDataJson);
            }

            return View("ProductAddEdit", products);
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProductModel modelProduct)
        {

            try
            {
                MultipartFormDataContent formData = new MultipartFormDataContent();
                formData.Add(new StringContent(modelProduct.CategoryID.ToString()), "CategoryID");
                formData.Add(new StringContent(modelProduct.ProductName), "ProductName");
                formData.Add(new StringContent(modelProduct.Price.ToString()), "Price");
                formData.Add(new StringContent(modelProduct.Description), "Description");
                formData.Add(new StringContent(modelProduct.StockQty.ToString()), "StockQty");
                formData.Add(new StringContent(modelProduct.Quantity.ToString()), "Quantity");
                formData.Add(new StringContent(modelProduct.Image), "Image");

                if (modelProduct.ProductID == null)
                {
                    HttpResponseMessage response = await _client.PostAsync($"{_client.BaseAddress}/Product/Insert", formData);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "Product Inserted";
                        return RedirectToAction("GET");
                    }
                }
                else
                {
                    HttpResponseMessage response = await _client.PutAsync($"{_client.BaseAddress}/Product/Update/?ProductID={modelProduct.ProductID}", formData);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "Product Updated";
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

        [HttpGet]
        public IActionResult GETCategory()
        {
            List<ProductModel> cats = new List<ProductModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Product/GetCategory").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonobject = JsonConvert.DeserializeObject(data);

                var dataofObject = jsonobject.data;
                var extractedDataJson = JsonConvert.SerializeObject(dataofObject, Formatting.Indented);
                cats = JsonConvert.DeserializeObject<List<ProductModel>>(extractedDataJson);
            }

            return View("ProductList", cats);
        }

    }
}
