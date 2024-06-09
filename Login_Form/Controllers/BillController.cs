using Humanizer;
using Login_Form.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;

namespace Login_Form.Controllers
{
    public class BillController : Controller
    {
        Models.AppContext _context;
        public BillController()
        {
            _context = new Models.AppContext();
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
                var allBill = _context.Bills.Where(bd => bd.Username == check).ToList();
                return View(allBill);
            }
        }

        // GET: BillController/Details/5
        public ActionResult Details(string id)
        {
            var billDetails = _context.BillDetails.Where(b => b.BillId == id).ToList();
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
                    Bill newdata = new Bill()
                    {
                        Id = bill.Id,
                        Description = bill.Description,
                        CreateDate = bill.CreateDate,
                        Username = check,
                        Status = bill.Status
                    };
                    _context.Bills.Add(newdata);
                    _context.SaveChanges();
                    var cartItems = _context.CartDetails.Where(p => p.Username == check).ToList();
                    foreach (var cartItem in cartItems)
                    {
                        var product = _context.Products.Find(cartItem.ProductId);
                        if(cartItem.Quantity > product.Quantity)
                        {
                            TempData["Error"] = $"Sản phẩm {cartItem.ProductId} không đủ số lượng!";
                            return RedirectToAction("Index", "Cart");
                        }
                        BillDetail billDetail = new BillDetail()
                        {
                            Id = cartItem.Id,
                            ProductId = cartItem.ProductId,
                            BillId = bill.Id,
                            Quantity = cartItem.Quantity,
                            Price = cartItem.Quantity * product.Price
                        };
                        _context.BillDetails.Add(billDetail);
                        _context.CartDetails.Remove(cartItem);
                        product.Quantity = product.Quantity - billDetail.Quantity;
                        _context.Products.Update(product);
                        _context.SaveChanges();
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
                var editData = _context.Bills.Find(id);
                editData.Status = 2; //Chuyển thành trạng thái đã xoá
                _context.Bills.Update(editData);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public IActionResult Refund(string id)
        {
            var bills = _context.BillDetails.Where(bd =>bd.BillId == id).ToList();
            if (_context.Bills.Find(id).Status != 100)
            {
                foreach (var item in bills)
                {
                    var product = _context.Products.First(p => p.Id == item.ProductId);
                    product.Quantity = product.Quantity + item.Quantity;
                    _context.Products.Update(product);
                    _context.SaveChanges();
                }
                var bill = _context.Bills.Find(id);
                bill.Status = 100;
                _context.Bills.Update(bill);
                _context.SaveChanges();
                TempData["RefundLog1"] = "Hoàn tiền thành công!";
            }
            else
            {
                TempData["RefundLog2"] = "Đơn hàng nãy đã được hoàn trả, bạn tính làm gì vậy?";
            }
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
                var billDetails = _context.BillDetails.Where(bd => bd.BillId == id).ToList();
                var productsLog1 = new List<string>();
                var productsLog2 = new List<string>();
                var AddSuccess1 = new List<string>();
                var AddSuccess2 = new List<string>();
                foreach (var item in billDetails)
                {
                    var cartDetail = _context.CartDetails.FirstOrDefault(p => p.ProductId == item.ProductId && p.Username == check);
                    if(cartDetail == null)
                    {
                        var product = _context.Products.First(p => p.Id == item.ProductId);
                        if (product.Quantity < item.Quantity)
                        {
                            productsLog1.Add(product.Id.ToString());
                        }
                        else
                        {
                            CartDetail cart = new CartDetail()
                            {
                                Id = Guid.NewGuid(),
                                ProductId = item.ProductId,
                                Username = check,
                                Quantity = item.Quantity,
                                Satus = 1
                            };
                            _context.CartDetails.Add(cart);
                            AddSuccess1.Add(cart.Id.ToString());
                        }
                        _context.SaveChanges();
                    }
                    else
                    {
                        var product = _context.Products.First(p => p.Id == item.ProductId);
                        if(product.Quantity < item.Quantity + cartDetail.Quantity)
                        {
                            productsLog2.Add(product.Id.ToString());
                        }
                        else
                        {
                            cartDetail.Quantity = item.Quantity + cartDetail.Quantity;
                            _context.CartDetails.Update(cartDetail);
                            AddSuccess2.Add(cartDetail.Id.ToString());
                        }
                        _context.SaveChanges();
                    }
                }
                if(productsLog1.Count > 0 || productsLog2.Count > 0)
                {
                    if(productsLog1.Count > 0)
                    {
                    TempData["BuyAgainLog1"] = $"Sản phẩm sau không đủ số lượng trong kho để thêm lại sản phẩm vào giỏ hàng: \n" +
                            $"{string.Join(", ", productsLog1.ToArray())}";
                    }

                    if(productsLog2.Count > 0)
                    {
                        TempData["BuyAgainLog2"] = $"Sản phẩm sau không đủ số lượng trong kho để cộng dồn sản phẩm vào giỏ hàng: \n" +
                                                    $"{string.Join(", ", productsLog2.ToArray())}";
                    }

                    if(AddSuccess1.Count > 0)
                    {
                        TempData["BillDetailsRestockLogSuccess1"] =
                            $"Sản phẩm sau đã được thêm vào : {string.Join(", ", AddSuccess1.ToArray())}";
                    }

                    if(AddSuccess2.Count > 0)
                    {
                        TempData["BillDetailsRestockLogSuccess2"] =
                            $"Sản phẩm sau đã được cộng thêm số lượng : {string.Join(", ", AddSuccess2.ToArray())}";
                    }

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
