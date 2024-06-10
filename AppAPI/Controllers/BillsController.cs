using AppAPI.Models;
using Login_Form.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillsController : ControllerBase
    {
        Login_Form.Models.AppContext _context;
        public BillsController()
        {
            _context = new Login_Form.Models.AppContext();
        }
        [HttpGet("get-all-bill")]
        public ActionResult Get(string check)
        {
            return Ok(_context.Bills.Where(bd => bd.Username == check).ToList());
        }
        [HttpGet("get-all-billdetails")]
        public ActionResult GetAction(string id)
        {
            return Ok(_context.BillDetails.Where(b => b.BillId == id).ToList());
        }
        [HttpPost("create-bill")]
        public ActionResult Create(string check ,Bills bill)
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
                if (cartItem.Quantity > product.Quantity)
                {
                    return Content($"Sản phẩm {cartItem.ProductId} không đủ số lượng!");
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
            return Ok();
        }
        [HttpDelete("delete-bill")]
        public ActionResult delete(int id)
        {
            try
            {
                var editData = _context.Bills.Find(id);
                editData.Status = 2; //Chuyển thành trạng thái đã xoá
                _context.Bills.Update(editData);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
        [HttpGet("refund")]
        public ActionResult refund(string id)
        {
            var bills = _context.BillDetails.Where(bd => bd.BillId == id).ToList();
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
                return Ok("Hoàn tiền thành công!");
            }
            else
            {
                return Content("Đơn hàng nãy đã được hoàn trả, bạn tính làm gì vậy?");
            }
        }
        [HttpGet("restock")]
        public ActionResult restock(string id, string check)
        {
            var productsLog1 = new List<string>();
            var billDetails = _context.BillDetails.Where(bd => bd.BillId == id).ToList();

            foreach (var item in billDetails)
            {
                var cartDetail = _context.CartDetails.FirstOrDefault(p => p.ProductId == item.ProductId && p.Username == check);
                if (cartDetail == null)
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
                    }
                    _context.SaveChanges();
                }
                else
                {
                    var product = _context.Products.First(p => p.Id == item.ProductId);
                    if (product.Quantity < item.Quantity + cartDetail.Quantity)
                    {
                        productsLog1.Add(product.Id.ToString());
                    }
                    else
                    {
                        cartDetail.Quantity = item.Quantity + cartDetail.Quantity;
                        _context.CartDetails.Update(cartDetail);
                    }
                    _context.SaveChanges();
                }
            }
            if (productsLog1.Count > 0)
            {
                return Content($"Sản phẩm bị lỗi khi restock {string.Join(", ", productsLog1.ToArray())}");
            }
            else
            {
                return Ok();
            }
        }
    }
}
