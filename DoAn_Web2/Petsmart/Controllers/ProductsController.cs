using System;
using System.Collections.Generic;
using Petsmart.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Petsmart.Controllers
{
    public class ProductsController : Controller
    {
        private ShopBanDongVatEntities db = new ShopBanDongVatEntities();
        // GET: Products
        public ActionResult Detail(int id)
        {
            ViewData["ChiTietSP"] = db.SanPhams.Where(s => s.MaSanPham == id).SingleOrDefault();
            var maloai = db.SanPhams.Where(s => s.MaSanPham == id).Select(s => s.MaLoaiSanPham);
            var masp = (ViewData["ChiTietSP"] as SanPham).MaSanPham;
            ViewData["SPTuongTu"] = db.SanPhams.Where(s => s.MaLoaiSanPham == maloai.FirstOrDefault() && s.MaSanPham != id).Select(s => s).ToList();
            ViewData["LoaiSP"] = db.LoaiSanPhams.Where(s => s.MaLoaiSanPham == maloai.FirstOrDefault()).SingleOrDefault();
            ViewData["HinhAnhSP"] = db.HinhAnhSanPhams.Where(s => s.MaSanPham == masp).ToList();
            //Đếm lượt view
            if (Request.Cookies["ViewProduct"] != null)
            {
                if (Request.Cookies["ViewProduct"][string.Format("pId_{0}", id)] == null)
                {
                    HttpCookie cookie = (HttpCookie)Request.Cookies["ViewProduct"];
                    cookie[string.Format("pId_{0}", id)] = "1";
                    cookie.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Add(cookie);
                    // Update lượt view
                    SanPham sp = db.SanPhams.SingleOrDefault(s=> s.MaSanPham == id);
                    sp.LuotXem += 1;
                    db.SaveChanges();
                }
            }
            else
            {
                HttpCookie cookie = new HttpCookie("ViewProduct");
                cookie[string.Format("pId_{0}", id)] = "1";
                cookie.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(cookie);

                SanPham sp = db.SanPhams.SingleOrDefault(s => s.MaSanPham == id);
                sp.LuotXem += 1;
                db.SaveChanges();
            }

            return View();
        }
    }
}