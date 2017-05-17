using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
using PagedList;
namespace Petsmart.Controllers
{
    public class AProductsController : Controller
    {
        private ShopBanDongVatEntities db = new ShopBanDongVatEntities();
        // GET: AProducts
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.PriceSortParm = String.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var sanphams = from s in db.SanPhams
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                sanphams = sanphams.Where(s => s.TenSanPham.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    sanphams = sanphams.OrderBy(s => s.TenSanPham);
                    break;
                case "date_desc":
                    sanphams = sanphams.OrderBy(s => s.NgayNhap);
                    break;
                case "id_desc":
                    sanphams = sanphams.OrderBy(s => s.MaSanPham);
                    break;
                case "price_desc":
                    sanphams = sanphams.OrderBy(s => s.GiaSanPham);
                    break;
                default:
                    sanphams = sanphams.OrderBy(s => s.NgayNhap);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(sanphams.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult LockProduct(int id)
        {
            SanPham sp = db.SanPhams.Single(s => s.MaSanPham == id);

            sp.BiXoa = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult UnlockProduct(int id)
        {
            SanPham sp = db.SanPhams.Single(s => s.MaSanPham == id);
            sp.BiXoa = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult AddProduct()
        {
            List<LoaiSanPham> lsp = db.LoaiSanPhams.ToList<LoaiSanPham>();
            ViewBag.LoaiSanPhams = new SelectList(lsp, "MaLoaiSanPham", "TenLoaiSanPham");
            List<HangSanXuat> hsx = db.HangSanXuats.ToList<HangSanXuat>();
            ViewBag.HangSanXuats = new SelectList(hsx, "MaHangSanXuat", "TenHangSanXuat");
            return View();
        }
        // Ajax show popover detail product
        [HttpPost]
        public string DetailProduct(int id)
        {
            SanPham sp = db.SanPhams.Single(s => s.MaSanPham == id);
            string img = "<img src='../Content/images/products/" + sp.HinhURL + "' class='img-circle img-sm' />";
            string out1 = img+"<div class='row'>" +
                                "<p>" +
                                    "<label>Tên:&nbsp;</label>" +
                                    "<strong>"+ sp.TenSanPham +"</strong>"+
                                "</p>" +
                                 "<p>" +
                                    "<label>Giá:&nbsp;</label>" +
                                    "<strong>" + String.Format("{0:0,0}", sp.GiaSanPham) + "đ</strong>" +
                                "</p>" +
                                 "<p>" +
                                    "<label>Thuộc loại:&nbsp;</label>" +
                                    "<strong>" + sp.LoaiSanPham.TenLoaiSanPham + "</strong>" +
                                "</p>"+
                                "<p>" +
                                    "<label>Đối tác:&nbsp;</label>" +
                                    "<strong>" + sp.HangSanXuat.TenHangSanXuat + "</strong>" +
                                "</p>" +
                                "<p>" +
                                    "<label>Số lượng bán:&nbsp;</label>" +
                                    "<strong>" + sp.SoLuongBan + "</strong>" +
                                "</p>" +
                                "<p>" +
                                    "<label>Lượt xem:&nbsp;</label>" +
                                    "<strong>" + sp.LuotXem + "</strong>" +
                                "</p>" +
                        "</div>";
            return out1;
        }
    }
}