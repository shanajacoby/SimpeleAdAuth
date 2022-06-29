using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleAdsAuth.Data;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace SimpleAdsAuth.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAd;Integrated Security=true;";

        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(User user, string password)
        {
            var repo = new AdsRepository(_connectionString);
            repo.AddUser(user, password);
            return Redirect("/");
        }
        public IActionResult Login()
        {
            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var repo = new AdsRepository(_connectionString);
            var user = repo.Login(email, password);
            if (user == null)
            {
                TempData["message"] = "Invalid Login!";
                return RedirectToAction("Login");
            }
            var claims = new List<Claim>
            {

                new Claim("user", email)
            };

            //The next line of code is the one that actually signs in the user
            //it basically sets a special cookie on the clients machine that
            //sets them as "logged in". Before using it though, make sure to add a
            //using on top:
            //using Microsoft.AspNetCore.Authentication;
            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            return Redirect("/home/newad");
        }


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }
    }
}
       

