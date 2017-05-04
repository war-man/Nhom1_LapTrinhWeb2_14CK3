using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
namespace Petsmart.Controllers
{
    public class HomeController : Controller
    {
        private ShopBanDongVatEntities db = new ShopBanDongVatEntities();
        public ActionResult Index()
        {   
            return View();
        }

        public PartialViewResult DanhMucHangDau()
        {
            List<SanPham> lstSanPham = db.SanPhams.OrderByDescending(s => s.LuotXem).Where(s => s.MaLoaiSanPham < 5).Select(s => s).ToList();
            return PartialView(lstSanPham);
        }

        public PartialViewResult SanPhamBanChay()
        {
            List<SanPham> lstSanPham = db.SanPhams.OrderByDescending(s => s.SoLuongBan).Select(s => s).Take(6).ToList();
            return PartialView(lstSanPham);
        }

        public PartialViewResult LogoThuongHieu()
        {
            List<HangSanXuat> lstHangSX = db.HangSanXuats.ToList();
            return PartialView(lstHangSX);
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }
    }
}