using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
using PagedList;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;

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
                    sanphams = sanphams.OrderByDescending(s => s.NgayNhap);
                    break;
                case "id_desc":
                    sanphams = sanphams.OrderByDescending(s => s.MaSanPham);
                    break;
                case "price_desc":
                    sanphams = sanphams.OrderBy(s => s.GiaSanPham);
                    break;
                default:
                    sanphams = sanphams.OrderByDescending(s => s.NgayNhap);
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
        [HttpPost]
        public string themsanpham(FormCollection fm)
        {
            SanPham sp = new SanPham();
            sp.TenSanPham = Convert.ToString(fm["tensanpham"].Trim());
            sp.SoLuongTon = Convert.ToInt32(fm["soluong"]);
            sp.GiaSanPham = Convert.ToDecimal(fm["giaban"]);
            sp.MaLoaiSanPham = Convert.ToInt32(fm["MaLoaiSanPham"]);
            sp.MaHangSanXuat = Convert.ToInt32(fm["MaHangSanXuat"]);
            sp.MoTa = Convert.ToString(fm["mota"].Trim());

            // default
            sp.LuotXem = 0;
            sp.NgayNhap = DateTime.Now;
            sp.SoLuongTon = 0;
            sp.BiXoa = false;

            // Thêm hình ảnh
            string json = Convert.ToString(fm["_images"]);
            List<ImagesProduct> lstImages = JsonConvert.DeserializeObject<List<ImagesProduct>>(json);

            // Lấy hình 1 đại diện mặc định khi user không chọn

            // Get ID MAX table SanPhams
            int idmax = 0;
            idmax = db.SanPhams.OrderByDescending(u => u.MaSanPham).FirstOrDefault().MaSanPham;
            idmax = idmax + 1;

            //GET ID MAX table HinhANHSANPHAM
            int idmax2 = 0;
            idmax2 = db.HinhAnhSanPhams.OrderByDescending(u => u.MaHinh).FirstOrDefault().MaHinh;
            var fileName ="";
            var path="";

            // kiểm tra xem đã đó lựa chọn chưa
            bool checkselected = false;
            foreach (var item in lstImages)
            {
                if(item.Status == true)
                {
                    checkselected = true;
                }
            }
            if(checkselected == false)
            {
                // chưa có thì mặc định là hình 1
                lstImages[0].Status = true;
            }
            // Save ảnh
             foreach (var item in lstImages)
            {
                if (lstImages.Count > 0)
                {
                    // Hình chọn làm ảnh đại diện
                    if(item.Status == true)
                    {
                        fileName = "p-"+ idmax.ToString() + "." + item.Extension ;
                        sp.HinhURL = fileName;
                        path = Path.Combine(Server.MapPath("~/Content/images/products"), fileName);
                        // Hình đại diện
                        Image image_root = item.Base64ToImage(item.Image);
                        image_root.Save(path);                        
                        db.SanPhams.Add(sp);
                    }
                    else
                    {
                        idmax2 = idmax2 + 1;
                        // Upload vào table HinhAnhSanPham
                        fileName = "slide-p-" + idmax.ToString() + "-" +idmax2.ToString() + "." + item.Extension;
                        path = Path.Combine(Server.MapPath("~/Content/images/products/slide_products"), fileName);
                        Image image = item.Base64ToImage(item.Image);
                        image.Save(path);
                        HinhAnhSanPham hssp = new HinhAnhSanPham();
                        hssp.MaSanPham = sp.MaSanPham;
                        hssp.HinhURL = fileName;
                        db.HinhAnhSanPhams.Add(hssp);
                    }
                }                
            }
            if (db.SaveChanges() > 0)
            {
                return "Thêm thành công";
            }
            else
            {
                return "Thêm thất bại";
            }
        }
    }
}