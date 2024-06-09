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
        public CartController()
        {
            _context = new Models.AppContext();
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
                var cartItems = _context.CartDetails.Where(p =>p.Username == check);
                decimal total = 0;
                cartItems.ToList().ForEach(p =>
                {
                    var product = _context.Products.Find(p.ProductId);
                    if(product != null)
                    {
                         total = p.Quantity * product.Price;
                    }
                });
                ViewBag.totalAmount = total.ToString("#,###",
                CultureInfo.GetCultureInfo("vi-VN").NumberFormat); ;
                return View(cartItems);
            }
        }

        public IActionResult Create()
        {
            return RedirectToAction("Index", "Product");
        }
        public IActionResult Details(Guid id)
        {
            var product = _context.CartDetails.Find(id);
            return View(product);
        }

        public IActionResult Edit(Guid id)
        {
            var editData = _context.CartDetails.Find(id);
            return View(editData);
        }

        [HttpPost]
        public IActionResult Edit(CartDetail cartDetail)
        {
            try
            {
                var editData = _context.CartDetails.Find(cartDetail.Id);
                if (cartDetail.Quantity > _context.Products.Find(cartDetail.ProductId).Quantity)
                {
                    TempData["CartEditError"] = $"Số lượng chỉ còn {_context.Products.Find(cartDetail.ProductId).Quantity}";
                    return RedirectToAction("Edit");
                }
                else { editData.Quantity = cartDetail.Quantity; }
                _context.CartDetails.Update(editData);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return Content("Có gì đó không đúng đã xảy ra!");
            }
        }

        public ActionResult Delete(Guid id)
        {
            var DeleteData = _context.CartDetails.Find(id);
            _context.CartDetails.Remove(DeleteData);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult CreateBill()
        {
            var check = HttpContext.Session.GetString("Account");
            var cartItems = _context.CartDetails.Where(p => p.Username == check).ToList();
            if (cartItems.Count == 0)
            {
                TempData["ProductLog"] = "Vui lòng mua sản phẩm trước khi thanh toán!";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                foreach (var item in cartItems) 
                {
                    if(_context.Products.Where(p=>p.Id == item.ProductId).First().Quantity == 0)
                    {
                        TempData["ErrorBill"] = $"Sản phẩm {item.ProductId} đã hết hàng!";
                        return RedirectToAction("Index");
                    }
                    else if(_context.Products.Where(p=>p.Id == item.ProductId).First().Quantity < item.Quantity)
                    {
                        TempData["ErrorBill"] = $"Sản phẩm {item.ProductId} trong kho không đáp ứng được số lượng trong giỏ hàng!";
                        return RedirectToAction("Index");
                    }
                }
                return RedirectToAction("Create", "Bill");
            }
            
        }
    }
}
