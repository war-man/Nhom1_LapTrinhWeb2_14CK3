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
    public class APartnerController : Controller
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

            var doitacs = from s in db.HangSanXuats
                          where s.BiXoa != true
                          select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                doitacs = doitacs.Where(s => s.TenHangSanXuat.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    doitacs = doitacs.OrderBy(s => s.TenHangSanXuat);
                    break;
                case "id_desc":
                    doitacs = doitacs.OrderByDescending(s => s.MaHangSanXuat);
                    break;
                default:
                    doitacs = doitacs.OrderByDescending(s => s.MaHangSanXuat);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(doitacs.ToPagedList(pageNumber, pageSize));
        }

        public string DeletePartner(string id)
        {
            int mhsx = Convert.ToInt32(id);

            int count = db.SanPhams.Where(s => s.MaHangSanXuat == mhsx).Count();

            if(count > 0)
            {
                return "exits_products";
            }
            else
            {
                // delete Hãng sản xuất
                HangSanXuat hsx = db.HangSanXuats.SingleOrDefault(s => s.MaHangSanXuat == mhsx);
                if(hsx == null)
                {
                    return "error_null";
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/Content/images/partner/"), hsx.LogoURL);
                    if ((System.IO.File.Exists(path)))
                    {
                        System.IO.File.Delete(path);
                    }
                    db.HangSanXuats.Remove(hsx);
                    db.SaveChanges();
                    return "success";
                }
            }
        }

        public string themdoitac(FormCollection fm)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase hinhdaidien = Request.Files["image"];
                if(hinhdaidien.FileName == "")
                {
                    return "error_images";
                }
                else
                {
                    HangSanXuat hsx = new HangSanXuat();
                    hsx.TenHangSanXuat = fm["tendoitac"];
                    hsx.BiXoa = false;
                    int idmax = 1;
                    if (db.HangSanXuats.Count() > 0)
                    {
                        idmax = db.HangSanXuats.OrderByDescending(u => u.MaHangSanXuat).FirstOrDefault().MaHangSanXuat;
                    }
                    string fileName,fname;// lưu rename tên file
                    FileInfo fi = new FileInfo(hinhdaidien.FileName);
                    string ext = fi.Extension;
                    idmax = idmax + 1;
                    // ảnh đại diện lưu vào folder partner
                    fileName = "par-" + idmax.ToString() + ext;
                    hsx.LogoURL = fileName;
                    hsx.MaHangSanXuat = idmax;
                    fname = Path.Combine(Server.MapPath("~/Content/images/partner/"), fileName);
                    hinhdaidien.SaveAs(fname);
                    db.HangSanXuats.Add(hsx);
                }
                if(db.SaveChanges() > 0)
                {
                    return "success";
                }
                else
                {
                    return "error";
                }
            }
            else
            {
                return "error_images";
            }
        }
        
        public JsonResult EditPartner(string id)
        {
            int madoitac = Convert.ToInt32(id);

            HangSanXuat hsx = db.HangSanXuats.SingleOrDefault(h => h.MaHangSanXuat == madoitac);
            if(hsx == null)
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
                HttpPostedFileBase hinhdaidien = Request.Files["image"];
                // thì ko upload hình chỉ cập nhật tên đối tác
                int id = Convert.ToInt32(fm["madoitac"]);
                HangSanXuat hsx = db.HangSanXuats.SingleOrDefault(s => s.MaHangSanXuat == id);
                if(hsx == null)
                {
                    return "error_product";
                }
                else
                {
                    if (hinhdaidien.FileName == "")
                    {
                        hsx.TenHangSanXuat = fm["tendoitac"];
                        db.SaveChanges();
                        return "success";
                    }
                    else
                    {
                        hsx.TenHangSanXuat = fm["tendoitac"];
                        hsx.BiXoa = false;
                        string fname;
                        // replace ảnh đại diện
                        fname = Path.Combine(Server.MapPath("~/Content/images/partner/"), hsx.LogoURL);
                        hinhdaidien.SaveAs(fname);
                        db.SaveChanges();
                        return "success";
                    }
                }
        }

    }
}