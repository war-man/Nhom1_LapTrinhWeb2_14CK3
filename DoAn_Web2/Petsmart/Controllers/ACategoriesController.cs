using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
using PagedList;
using System.IO;

namespace Petsmart.Controllers
{
    public class ACategoriesController : Controller
    {
        private ShopBanDongVatEntities db = new ShopBanDongVatEntities();

        // GET: 
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var loaisps = from s in db.LoaiSanPhams
                          where s.BiXoa != true
                          select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                loaisps = loaisps.Where(s => s.TenLoaiSanPham.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    loaisps = loaisps.OrderBy(s => s.TenLoaiSanPham);
                    break;
                case "id_desc":
                    loaisps = loaisps.OrderByDescending(s => s.MaLoaiSanPham);
                    break;
                default:
                    loaisps = loaisps.OrderByDescending(s => s.MaLoaiSanPham);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(loaisps.ToPagedList(pageNumber, pageSize));
        }

        public string DeletePartner(string id)
        {
            int mhsx = Convert.ToInt32(id);

            int count = db.SanPhams.Where(s => s.MaLoaiSanPham == mhsx).Count();

            if (count > 0)
            {
                return "exits_products";
            }
            else
            {
                // delete Hãng sản xuất
                LoaiSanPham hsx = db.LoaiSanPhams.SingleOrDefault(s => s.MaLoaiSanPham == mhsx);
                if (hsx == null)
                {
                    return "error_null";
                }
                else
                {
                    
                    db.LoaiSanPhams.Remove(hsx);
                    db.SaveChanges();
                    return "success";
                }
            }
        }

        public string themdoitac(FormCollection fm)
        {
            LoaiSanPham hsx = new LoaiSanPham();
            hsx.TenLoaiSanPham = fm["tendoitac"];
            hsx.BiXoa = false;
            db.LoaiSanPhams.Add(hsx);
            if (db.SaveChanges() > 0)
            {
                return "success";
            }
            else
            {
                return "error";
            }
        }
        [HttpPost]
        public JsonResult EditPartner(string id)
        {
            int madoitac = Convert.ToInt32(id);
            db.Configuration.ProxyCreationEnabled = false;
            LoaiSanPham hsx = db.LoaiSanPhams.Single(h => h.MaLoaiSanPham == madoitac);
            if (hsx == null)
            {
                return Json("Errorhsx");
            }
            else
            {
                return Json(hsx, JsonRequestBehavior.AllowGet);
            }
        }
        public string suadoitac(FormCollection fm)
        {
            int id = Convert.ToInt32(fm["madoitac"]);
            LoaiSanPham hsx = db.LoaiSanPhams.SingleOrDefault(s => s.MaLoaiSanPham == id);
            hsx.TenLoaiSanPham = fm["tendoitac"];
            db.SaveChanges();
            return "success";
        }

    }
}