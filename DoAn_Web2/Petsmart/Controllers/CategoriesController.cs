using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
using PagedList;
namespace Petsmart.Controllers
{
    public class CategoriesController : Controller
    {

        private ShopBanDongVatEntities db = new ShopBanDongVatEntities();
        // GET: Categories
        public ActionResult Index(int id ,string sortOrder, string currentFilter, int? page)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParm = String.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            List<SanPham> sanpham = new List<SanPham>();
            ViewBag.CurrentSort = sortOrder;
            switch (sortOrder)
            {
                case "name_desc":
                    sanpham = db.SanPhams.Where(s=> s.MaLoaiSanPham == id).OrderByDescending(s => s.TenSanPham).ToList();
                    break;
                case "price_desc":
                    sanpham = db.SanPhams.Where(s => s.MaLoaiSanPham == id).OrderByDescending(s => s.GiaSanPham).ToList(); ;
                    break;
                case "date_desc":
                    sanpham = db.SanPhams.Where(s => s.MaLoaiSanPham == id).OrderByDescending(s => s.NgayNhap).ToList();
                    break;
                default:
                    sanpham = db.SanPhams.Where(s => s.MaLoaiSanPham == id).OrderByDescending(s => s.TenSanPham).ToList();
                    break;
            }
            // slider trai
            ViewData["lstLoaiSanPham"] = db.LoaiSanPhams.ToList();
            ViewData["lsp"] = db.LoaiSanPhams.Where(s => s.MaLoaiSanPham == id).ToList();
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(sanpham.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult List()
        {
            return View();
        }
    }
}