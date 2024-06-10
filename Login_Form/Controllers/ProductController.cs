using Login_Form.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Login_Form.Controllers
{
    public class ProductController : Controller
    {
        Models.AppContext _context;
        HttpClient _client;
        public ProductController()
        {
            _context = new Models.AppContext();
            _client = new HttpClient();
        }
        // GET: ProductController
        public ActionResult Index() // Lấy ra danh sách sản phẩm
        {
            var url = @"https://localhost:7137/api/Products/get-all-product";
            var result = JsonConvert.DeserializeObject<List<Product>>(
                _client.GetStringAsync(url).Result);
            return View(result);
        }

        // GET: ProductController/Details/5
        public ActionResult Details(Guid id)
        {
            var url = $@"https://localhost:7137/api/Products/get-by-id-product?id={id}";
            var result = JsonConvert.DeserializeObject<Product>(
                _client.GetStringAsync(url).Result);
            return View(result);
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            Product fakedata = new Product() //tạo 1 chút thông tin để điền sẵn sang
            {
                Id = Guid.NewGuid(),
                Name = "Sản phẩm mẫu",
                Description = " Tươi ngol bổ rẻ",
                Quantity = new Random().Next(1, 500),
                Price = new Random().Next(10000, 1000000), //Random giá trị từ 10000 đến 1000000
                Status = 1
            };
            return View(fakedata);
        }

        // POST: ProductController/Create
        [HttpPost]
        public ActionResult Create(Product product)
        {
			var check = HttpContext.Session.GetString("Account");
			if (string.IsNullOrEmpty(check))
			{
				return RedirectToAction("Login", "Account");
			}
            else
            {
                try
                {
                    string url = @"https://localhost:7137/api/Products/create-product";
                    var response = _client.PostAsJsonAsync(url, product).Result;
                    return RedirectToAction("Index");
                }
                catch
                {
                    return Content("Đã có lỗi xảy ra!");
                }
            }
				
        }

        // GET: ProductController/Edit/5
        public ActionResult Edit(Guid id)
        {
            //Lấy được thông tin cần sửa lên form sửa
            var url = $@"https://localhost:7137/api/Products/get-by-id-product?id={id}";
            var result = JsonConvert.DeserializeObject<Product>(
                _client.GetStringAsync(url).Result);
            return View(result);
        }
        [HttpPost]
        public ActionResult Edit(Product product)
        {
			var check = HttpContext.Session.GetString("Account");
			if (string.IsNullOrEmpty(check))
			{
				return RedirectToAction("Login", "Account");
			}
            else
            {
                try
                {

                    var url = @"https://localhost:7137/api/Products/update-product";
                    var response = _client.PutAsJsonAsync(url, product).Result;
                    return RedirectToAction("Index");
                }
                catch
                {
                    return Content("Đâu phải lỗi lầm nào cũng sửa được!");
                }
            }
				
        }

        // GET: ProductController/Delete/5
        public ActionResult Delete(Guid id)
        {
			var check = HttpContext.Session.GetString("Account");
			if (string.IsNullOrEmpty(check))
			{
				return RedirectToAction("Login", "Account");
			}
            else
            {
                var url = $@"https://localhost:7137/api/Products/delete-product?id={id}";
                var response = _client.DeleteAsync(url).Result;
                return RedirectToAction("Index");
            }
				
        }

        //Thêm sản phẩm vào giỏ hàng
        public IActionResult AddToCart(Guid id, int quantity) 
        {
            //Check đăng nhập
            var check = HttpContext.Session.GetString("Account");
            if(string.IsNullOrEmpty(check))
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var url = $@"https://localhost:7137/api/Products/add-to-cart?check={check}&id={id}&quantity={quantity}";
                var response = _client.GetStringAsync(url).Result;
                if (string.IsNullOrEmpty(response)) return RedirectToAction("Index","Product");
                TempData["ProductLog"] = response;
                return RedirectToAction("Index", "Product");
            }
        }
    }
}
