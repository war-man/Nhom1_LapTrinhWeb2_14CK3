using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
namespace Petsmart.Controllers
{
    public class CategoriesController : Controller
    {

        private ShopBanDongVatEntities db = new ShopBanDongVatEntities();
        // GET: Categories
        public ActionResult Index(int id)
        {
            ViewData["lstSanPham"] = db.SanPhams.Where(s => s.MaLoaiSanPham == id).ToList();
            ViewData["lstLoaiSanPham"] = db.LoaiSanPhams.ToList();
            ViewData["lsp"] = db.LoaiSanPhams.Where(s => s.MaLoaiSanPham == id).ToList();
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
    }
}