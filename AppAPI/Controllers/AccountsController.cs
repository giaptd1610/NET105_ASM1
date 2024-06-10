using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Login_Form.Models;
using System;
using System.Security.Principal;
using AppAPI.Models;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        Login_Form.Models.AppContext appContext;

        public AccountsController()
        {
            appContext = new Login_Form.Models.AppContext();
        }
        [HttpGet("get-all-account")]
        public IActionResult getall()
        {
            return Ok(appContext.Accounts.ToList());
        }
        [HttpPost("signup")]
        public IActionResult SignUp(Accounts account)
        {
            try
            {
                Account acc = new Account()
                {
                    Username = account.Username,
                    Password = account.Password,
                    Email = account.Email,
                    Phone = account.Phone,
                    Address = account.Address,
                };
                appContext.Accounts.Add(acc);
                Cart cart = new Cart() //Tạo 1 cart mới cho mỗi user được tạo
                {
                    Username = account.Username,
                    Status = 1
                };
                appContext.Carts.Add(cart);
                appContext.SaveChanges();
                return Ok(); // Chuyển hướng về login
            }
            catch
            {
                return BadRequest();
            }
        }
        
    }
}
