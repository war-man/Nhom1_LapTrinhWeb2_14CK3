using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
using PagedList;
namespace Petsmart.Controllers
{
    public class HomeController : Controller
    {
        private ShopBanDongVatEntities db = new ShopBanDongVatEntities();
        public ActionResult Index()
        {
            ViewData["lstDanhMuc"] = db.SanPhams.OrderByDescending(s => s.LuotXem).Where(s => s.MaLoaiSanPham < 5).Select(s => s).ToList();
            ViewData["lstSanPham"] = db.SanPhams.OrderByDescending(s => s.SoLuongBan).Select(s => s).Take(6).ToList();
            ViewData["lstNSX"] = db.HangSanXuats.ToList();

            return View();
        }
        public ActionResult Search(string sortOrder, string currentFilter, string searchString, int? page)
        {
            List<SanPham> sanpham = new List<SanPham>();
            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                sanpham = db.SanPhams.Where(s => s.TenSanPham.Contains(searchString)).ToList();

            }
            
            ViewData["lstLoaiSanPham"] = db.LoaiSanPhams.ToList();
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(sanpham.ToPagedList(pageNumber, pageSize));
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