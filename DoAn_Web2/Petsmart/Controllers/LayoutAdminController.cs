using Petsmart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Petsmart.Controllers
{
    public class LayoutAdminController : Controller
    {
        ShopBanDongVatEntities db = new ShopBanDongVatEntities();
        // GET: LayoutAdmin
        public PartialViewResult getInfoShopLayout()
        {
            var tt = db.ThongTinShops.SingleOrDefault(t => t.ID == 1);
            return PartialView(tt);
        }
    }
}