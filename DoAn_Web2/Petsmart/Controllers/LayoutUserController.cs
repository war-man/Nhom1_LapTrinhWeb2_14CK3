using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
namespace Petsmart.Controllers
{
    public class LayoutUserController : Controller
    {
       ShopBanDongVatEntities db = new ShopBanDongVatEntities();
        // GET: LayoutUser
      public PartialViewResult getMenu()
      {
            var LoaiSanPham = db.LoaiSanPhams.ToList();
            return PartialView(LoaiSanPham);
      }
    }
}