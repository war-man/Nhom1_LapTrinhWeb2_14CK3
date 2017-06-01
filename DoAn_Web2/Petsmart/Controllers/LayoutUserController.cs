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
      public PartialViewResult getmenuLoaiSanPham()
      {
            var LoaiSanPham = db.LoaiSanPhams.ToList();
            return PartialView(LoaiSanPham);
      }
      public PartialViewResult getmenuHangSanXuat()
      {
            var HangSanXuat = db.HangSanXuats.ToList();
            return PartialView(HangSanXuat);
        }
        // get slide left loai san pham
        public PartialViewResult leftSideLoaiSanPham()
        {
            var LoaiSanPham = db.LoaiSanPhams.ToList();
            return PartialView(LoaiSanPham);
        }
        public PartialViewResult leftSideHangSanXuat()
        {
            var HangSanXuat = db.HangSanXuats.ToList();
            return PartialView(HangSanXuat);
        }

    }
}