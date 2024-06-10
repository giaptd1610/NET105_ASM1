using AppAPI.Models;
using Login_Form.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        Login_Form.Models.AppContext _context;
        public CartsController()
        {
            _context = new Login_Form.Models.AppContext();
        }
        [HttpGet("get-all-cartDetails")]
        public ActionResult Get(string check)
        {
            var cartItems = _context.CartDetails.Where(p => p.Username == check);
            return Ok(cartItems.ToList());
        }
        [HttpGet("get-by-id-cartDetails")]
        public ActionResult getById(Guid id)
        {
            var product = _context.CartDetails.Find(id);
            return Ok(product);
        }
        [HttpPut("update-cartDetail")]
        public ActionResult Put(CartDetails cartDetail)
        {
            var editData = _context.CartDetails.Find(cartDetail.Id);
            if (cartDetail.Quantity > _context.Products.Find(cartDetail.ProductId).Quantity)
            {
                return Content($"Số lượng chỉ còn {_context.Products.Find(cartDetail.ProductId).Quantity}");
            }
            else { editData.Quantity = cartDetail.Quantity; }
            _context.CartDetails.Update(editData);
            _context.SaveChanges();
            return Ok();
        }
        [HttpDelete("delete-cartDetail")]
        public ActionResult Delete(Guid id)
        {
            var DeleteData = _context.CartDetails.Find(id);
            _context.CartDetails.Remove(DeleteData);
            _context.SaveChanges();
            return Ok();
        }
        [HttpGet("create-bill")]
        public IActionResult CreateBill(string check)
        {
            var cartItems = _context.CartDetails.Where(p => p.Username == check).ToList();
            if (cartItems.Count == 0)
            {
                return Content("Vui lòng mua sản phẩm trước khi thanh toán!");
            }
            else
            {
                foreach (var item in cartItems)
                {
                    if (_context.Products.Where(p => p.Id == item.ProductId).First().Quantity == 0)
                    {
                        return Content($"Sản phẩm {item.ProductId} đã hết hàng!");
                    }
                    else if (_context.Products.Where(p => p.Id == item.ProductId).First().Quantity < item.Quantity)
                    {
                        return Content($"Sản phẩm {item.ProductId} trong kho không đáp ứng được số lượng trong giỏ hàng!");
                    }
                }
                return Ok();
            }
        }
    }
}
