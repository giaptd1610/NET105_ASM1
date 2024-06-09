using Login_Form.Models;
using Microsoft.AspNetCore.Mvc;

namespace Login_Form.Controllers
{
    public class AccountController : Controller
    {
        Models.AppContext appContext;
        public AccountController()
        {
            appContext = new Models.AppContext();
        }
        public IActionResult Login(string username, string password)
        {
            if(username == null && password == null)
            {
                return View();
            }
            else
            {
                //Kiểm tra dữ liệu đăng nhập và trả về kết quả
                var data = appContext.Accounts.FirstOrDefault(p => p.Username == username && p.Password == password);
                if(data == null)
                {
                    return Content("Đăng nhập thất bại");
                }
                else
                {
                    HttpContext.Session.SetString("Account", username); //Gắn dữ liệu đăng nhập vào session
                    return RedirectToAction("Index", "Home"); //Nếu thành công thì điều hướng về home
                }
            }
        }

        public IActionResult SignUp() //Chỉ để mở view
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(Account account)
        {
            try
            {
                appContext.Accounts.Add(account);
                Cart cart = new Cart() //Tạo 1 cart mới cho mỗi user được tạo
                {
                    Username = account.Username,
                    Status = 1
                };
                appContext.Carts.Add(cart);
                appContext.SaveChanges();
                TempData["Status"] = "Đăng kí thành công!"; //Tạo thông báo
                return RedirectToAction("Login"); // Chuyển hướng về login
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
