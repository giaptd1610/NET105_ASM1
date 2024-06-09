using Login_Form.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Login_Form.Controllers
{
    public class ProductController : Controller
    {
        Models.AppContext _context;
        public ProductController()
        {
            _context = new Models.AppContext();
        }
        // GET: ProductController
        public ActionResult Index() // Lấy ra danh sách sản phẩm
        {
            var allProducts = _context.Products.ToList();
            return View(allProducts);
        }

        // GET: ProductController/Details/5
        public ActionResult Details(Guid id)
        {
            var product = _context.Products.Find(id); //Chỉ áp dụng cho PK
            return View(product);
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
                    _context.Products.Add(product); _context.SaveChanges();
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
            var editData = _context.Products.Find(id);
            return View(editData);
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
                    var editData = _context.Products.Find(product.Id); //tìm ra đối tượng cần sửa
                    editData.Name = product.Name;
                    editData.Description = product.Description;
                    editData.Price = product.Price;
                    editData.Quantity = product.Quantity;
                    editData.Status = product.Status;
                    _context.Products.Update(editData);
                    _context.SaveChanges();
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
                var DeleteData = _context.Products.Find(id);
                _context.Products.Remove(DeleteData);
                _context.SaveChanges();
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
                //Xem trong giỏ hàng ứng với user đó đã có sản phẩm với id này hay chưa
                var cartItem = _context.CartDetails.FirstOrDefault(p=>p.ProductId == id && p.Username == check);
                if(cartItem == null)//Sản phẩm chưa hề tồn tại trong giỏ hàng
                {
                    if(quantity >= _context.Products.Find(id).Quantity)
                    {
                        quantity = _context.Products.Find(id).Quantity;
                    }
                    CartDetail cartDetail = new CartDetail() // Tạo mới 1 CartDetails
                    {
                        Id = Guid.NewGuid(),
                        Username = check,
                        ProductId = id,
                        Quantity = quantity,
                        Satus = 1
                    };
                    _context.CartDetails.Add(cartDetail);
                    _context.SaveChanges();
                }
                else
                {
                    if (quantity >= _context.Products.Find(id).Quantity)
                    {
                        cartItem.Quantity = _context.Products.Find(id).Quantity;
                    }else if (_context.Products.Find(id).Quantity == 0)
                    {
                        TempData["ProductLog"] = "Sản phẩm đã hết hàng";
                        return RedirectToAction("Index", "Product");
                    }else if (cartItem.Quantity + quantity > _context.Products.Find(id).Quantity)
                    {
                        TempData["ProductLog"] = "Giỏ hàng của bạn đã chứa tối đa sản phẩm trong kho!";
                        return RedirectToAction("Index", "Product");
                    }
                    else
                    {
                        cartItem.Quantity = cartItem.Quantity + quantity; //Cộng dồn số lượng, đã check quá tổng
                    }
                    _context.CartDetails.Update(cartItem);
                    _context.SaveChanges();
                }
                return RedirectToAction("Index","Product");//   Quay lại trang danh sách sản phẩm
            }
        }
    }
}
