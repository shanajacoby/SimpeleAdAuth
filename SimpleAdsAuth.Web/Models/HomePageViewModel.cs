using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleAdsAuth.Data;

namespace SimpleAdsAuth.Web.Models
{
    public class HomePageViewModel
    {
        public bool IsAuthenticated { get; set; }
        public User CurrentUser { get; set; }
        public List<Ad> Ads { get; set; }
    }
}
