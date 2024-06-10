using Login_Form.Models;
using Microsoft.AspNetCore.Mvc;

namespace Login_Form.Controllers
{
    public class AccountController : Controller
    {
        Models.AppContext appContext;
        HttpClient _client;
        public AccountController()
        {
            appContext = new Models.AppContext();
            _client = new HttpClient();
        }
        public IActionResult Login(string username, string password)
        {
            if (username == null && password == null)
            {
                return View();
            }
            else
            {
                //Kiểm tra dữ liệu đăng nhập và trả về kết quả
                var data = appContext.Accounts.FirstOrDefault(p => p.Username == username && p.Password == password);
                if (data == null)
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
            var url = @"https://localhost:7137/api/Accounts/signup";
            var response = _client.PostAsJsonAsync(url, account).Result;
            if (response.IsSuccessStatusCode) return RedirectToAction("Login");
            return BadRequest();
        }
    }
}
