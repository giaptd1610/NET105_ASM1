using AppAPI.Models;
using Login_Form.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        Login_Form.Models.AppContext _context;
        public ProductsController()
        {
            _context = new Login_Form.Models.AppContext();
        }
        [HttpGet("get-all-product")]
        public ActionResult get()
        {
            return Ok(_context.Products.ToList());
        }
        [HttpGet("get-by-id-product")]
        public ActionResult get(Guid id)
        {
            return Ok(_context.Products.Find(id));
        }
        [HttpPost("create-product")]
        public ActionResult create(Products product)
        {
            Product data = new Product()
            {
                Id = Guid.NewGuid(),
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Status = product.Status,
            };
            _context.Products.Add(data); 
            _context.SaveChanges();
            return Ok();
        }
        [HttpPut("update-product")]
        public ActionResult update(Products product)
        {
            var editData = _context.Products.Find(product.Id); //tìm ra đối tượng cần sửa
            editData.Name = product.Name;
            editData.Description = product.Description;
            editData.Price = product.Price;
            editData.Quantity = product.Quantity;
            editData.Status = product.Status;
            _context.Products.Update(editData);
            _context.SaveChanges(); 
            return Ok();
        }
        [HttpDelete("delete-product")]
        public ActionResult delete(Guid id)
        {
            var DeleteData = _context.Products.Find(id);
            _context.Products.Remove(DeleteData);
            _context.SaveChanges();
            return Ok();
        }
        [HttpGet("add-to-cart")]
        public ActionResult AddToCart(string check, Guid id, int quantity)
        {
            //Xem trong giỏ hàng ứng với user đó đã có sản phẩm với id này hay chưa
            var cartItem = _context.CartDetails.FirstOrDefault(p => p.ProductId == id && p.Username == check);
            if (cartItem == null && _context.Products.Find(id).Quantity > 0)//Sản phẩm chưa hề tồn tại trong giỏ hàng
            {
                if (quantity >= _context.Products.Find(id).Quantity)
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
                return Ok();
            }
            else
            {
                if (_context.Products.Find(id).Quantity == 0)
                {
                    return Content("Sản phẩm đã hết hàng");
                }
                else if (quantity >= _context.Products.Find(id).Quantity)
                {
                    cartItem.Quantity = _context.Products.Find(id).Quantity;
                }
                else if (cartItem.Quantity + quantity > _context.Products.Find(id).Quantity)
                {
                    return Content("Giỏ hàng của bạn đã chứa tối đa sản phẩm trong kho!");
                }
                else
                {
                    cartItem.Quantity = cartItem.Quantity + quantity; //Cộng dồn số lượng, đã check quá tổng
                }
                _context.CartDetails.Update(cartItem);
                _context.SaveChanges();
                return Ok();
            }
            
            //   Quay lại trang danh sách sản phẩm
        }
    }
}
