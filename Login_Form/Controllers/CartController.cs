using Login_Form.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;

namespace Login_Form.Controllers
{
    public class CartController : Controller
    {
        Models.AppContext _context;
        HttpClient _httpClient;
        public CartController()
        {
            _context = new Models.AppContext();
            _httpClient = new HttpClient();
        }
        public IActionResult Index()
        {
            var check = HttpContext.Session.GetString("Account");
            if(string.IsNullOrEmpty(check) )
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var url = $@"https://localhost:7137/api/Carts/get-all-cartDetails?check={check}";
                var response = _httpClient.GetStringAsync(url).Result;
                var cartItems = JsonConvert.DeserializeObject<List<CartDetail>>(response);
                decimal total = 0;
                cartItems.ToList().ForEach(p =>
                {
                    var product = _context.Products.Find(p.ProductId);
                    if (product != null)
                    {
                        total = p.Quantity * product.Price;
                    }
                });
                ViewBag.totalAmount = total.ToString("#,###",
                CultureInfo.GetCultureInfo("vi-VN").NumberFormat);
                return View(cartItems);
            }
        }

        public IActionResult Create()
        {
            return RedirectToAction("Index", "Product");
        }
        public IActionResult Details(Guid id)
        {
            var url = $@"https://localhost:7137/api/Carts/get-by-id-cartDetails?id={id}";
            var response = _httpClient.GetStringAsync(url).Result;
            var product = JsonConvert.DeserializeObject<CartDetail>(response);
            return View(product);
        }

        public IActionResult Edit(Guid id)
        {
            var url = $@"https://localhost:7137/api/Carts/get-by-id-cartDetails?id={id}";
            var response = _httpClient.GetStringAsync(url).Result;
            var product = JsonConvert.DeserializeObject<CartDetail>(response);
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(CartDetail cartDetail)
        {
            try
            {
                var url = $@"https://localhost:7137/api/Carts/update-cartDetail";
                var response = _httpClient.PutAsJsonAsync(url, cartDetail).Result;
                if (!response.IsSuccessStatusCode)
                {
                    TempData["CartEditError"] = response.ToString();
                    return RedirectToAction("Edit");
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return Content("Có gì đó không đúng đã xảy ra!");
            }
        }

        public ActionResult Delete(Guid id)
        {
            var url = $@"https://localhost:7137/api/Carts/delete-cartDetail?id={id}";
            var response = _httpClient.DeleteAsync(url).Result;
            return RedirectToAction("Index");
        }

        public IActionResult CreateBill()
        {
            var check = HttpContext.Session.GetString("Account");
            var url = $@"https://localhost:7137/api/Carts/create-bill?check={check}";
            var response = _httpClient.GetStringAsync(url).Result;
            if (response.ToString() == "Vui lòng mua sản phẩm trước khi thanh toán!")
            {
                TempData["ProductLog"] = response;
                return RedirectToAction("Index", "Product");
            }
            if (!string.IsNullOrEmpty(response))
            {
                TempData["ErrorBill"] = response;
                return RedirectToAction("Index");
            }
            return RedirectToAction("Create", "Bill");
        }
    }
}
