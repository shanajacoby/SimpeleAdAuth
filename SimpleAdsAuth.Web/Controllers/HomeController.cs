using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleAdsAuth.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SimpleAdsAuth.Data;
using Microsoft.AspNetCore.Authorization;

namespace SimpleAdsAuth.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAd;Integrated Security=true;";
        public IActionResult Index()
        {
            var repo = new AdsRepository(_connectionString);
        
            var vm = new HomePageViewModel
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                Ads= repo.GetAllAds()
            };
            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name;
                vm.CurrentUser = repo.GetByEmail(email);
            }
            return View(vm);
        }
        [Authorize]
        public IActionResult NewAd()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("Account/login");
            }
            return View();
        }
        [HttpPost]
        
        public IActionResult NewAd(Ad ad)
        {
            var repo = new AdsRepository(_connectionString);
           
            var email = User.Identity.Name;
            var user = repo.GetByEmail(email);
            ad.UserName = user.Name;
            ad.UserId = user.UserId;
            repo.NewAd(ad);
            return Redirect("/");
        }
        [HttpPost]
        public IActionResult DeleteAd(int adId)
        {
            var repo = new AdsRepository(_connectionString);
            repo.DeleteAd(adId);
            return Redirect("/");
        }
        [Authorize]
        public IActionResult MyAccount()
        {
            return View();
        }




    }
}
