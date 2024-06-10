using Humanizer;
using Login_Form.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Globalization;

namespace Login_Form.Controllers
{
    public class BillController : Controller
    {
        Models.AppContext _context;
        HttpClient _httpClient;
        public BillController()
        {
            _context = new Models.AppContext();
            _httpClient = new HttpClient();
        }
        // GET: BillController
        public ActionResult Index()
        {
            var check = HttpContext.Session.GetString("Account");
            if(check== null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var url = $@"https://localhost:7137/api/Bills/get-all-bill?check={check}";
                var response = _httpClient.GetStringAsync(url).Result;
                var allBill = JsonConvert.DeserializeObject<List<Bill>>(response);
                return View(allBill);
            }
        }

        // GET: BillController/Details/5
        public ActionResult Details(string id)
        {
            var url = $@"https://localhost:7137/api/Bills/get-all-billdetails?id={id}";
            var response = _httpClient.GetStringAsync(url).Result;
            var data = JsonConvert.DeserializeObject<List<BillDetail>>(response);
            var billDetails = data.Where(b => b.BillId == id).ToList();
            ViewBag.totalAmount = billDetails.Sum(p => p.Price).ToString("#,###", 
                CultureInfo.GetCultureInfo("vi-VN").NumberFormat);
            ViewBag.billId = id;
            return View(billDetails);
        }

        // GET: BillController/Create
        public ActionResult Create()
        {
            var check = HttpContext.Session.GetString("Account");
            Bill newdata = new Bill()
            {
                Id = Guid.NewGuid().ToString(),
                CreateDate = DateTime.Now,
                Username = check,
                Status = 1
            };
            return View(newdata);
        }

        // POST: BillController/Create
        [HttpPost]
        public ActionResult Create(Bill bill)
        {
            try
            {
                var check = HttpContext.Session.GetString("Account");
                if (check == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    var url = $@"https://localhost:7137/api/Bills/create-bill?check={check}";
                    var response = _httpClient.PostAsJsonAsync(url, bill).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["ErrorBill"] = response.ToString();
                        return RedirectToAction("Cart", "Index");
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // POST: BillController/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: BillController/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var url = $@"https://localhost:7137/api/Bills/delete-bill?id={id}";
                var response = _httpClient.DeleteAsync(url).Result;
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public IActionResult Refund(string id)
        {
            var url = $@"https://localhost:7137/api/Bills/refund?id={id}";
            var response = _httpClient.GetStringAsync(url).Result;
            TempData["RefundLog1"] = response;
            return RedirectToAction("Index");
        }

        public IActionResult Restock(string id)
        {
            var check = HttpContext.Session.GetString("Account");
            if (check == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var url = $@"https://localhost:7137/api/Bills/restock?id={id}&check={check}";
                var response = _httpClient.GetStringAsync(url).Result;
                if (!string.IsNullOrEmpty(response))
                {
                    TempData["BuyAgainLog1"] = response;
                    return RedirectToAction("Details", new { id });
                }
				else
                {
                    TempData["CreateAgain"] = "Restock thành công!";
                    return RedirectToAction("Index", "Cart");
                }
            }
        }
    }
}
