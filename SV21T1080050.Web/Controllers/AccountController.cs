using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1080050.BusinessLayers;

namespace SV21T1080050.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.Username = username;

            //Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu");
                return View();
            }    

            //TODO: Kiểm tra xem username và password có đúng không?
            //if (username != "admin")
            //{
            //    ModelState.AddModelError("Error", "Đăng nhập thất bại");
            //    return View();
            //}

            var userAccount = UserAccountService.Authorize(UserTypes.Employee, username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }    

            //Đăng nhập thành công

            //1.Tạo thông tin người dùng 
            var userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList()//tách chuỗi đó thành 1 danh sách
            };


            //2.Ghi nhận trạng thái đăng nhập( trả cookie về cho người sử dụng)
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());

            //3.Quay về trang chủ
            return RedirectToAction("Index", "Home");

        }
        public async Task<IActionResult> Logout()
        {
           HttpContext.Session.Clear();
           await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
           return RedirectToAction("Login");
        }
        public IActionResult ChangePassword()
        {
            return View();
        }

        public IActionResult AccessDenined()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

    }
}
