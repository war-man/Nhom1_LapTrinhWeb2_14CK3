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

        [HttpPost]
        public string themsanpham(FormCollection fm)
        {

            if (Request.Files.Count > 0)
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
                sp.BiXoa = false;
                sp.SoLuongBan = 0;
                try
                {
                    // get hình đại diện
                    HttpPostedFileBase hinhdaidien = Request.Files["image"];
                    //  Get all files from Request object  
                    string fname;
                    int idmax = 1;
                    int idmax2 = 0;
                    if (db.HinhAnhSanPhams.Count() > 0)
                        idmax2 = db.HinhAnhSanPhams.OrderByDescending(u => u.MaHinh).FirstOrDefault().MaHinh;

                    if (db.SanPhams.Count() > 0)
                    {
                        idmax = db.SanPhams.OrderByDescending(u => u.MaSanPham).FirstOrDefault().MaSanPham;
                    }

                    HttpFileCollectionBase files = Request.Files;

                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fileName;// lưu rename tên file
                        if (file.FileName == hinhdaidien.FileName)
                        {
                            // lưu hình đại diện
                            idmax = idmax + 1;
                            FileInfo fi = new FileInfo(file.FileName);
                            string ext = fi.Extension;
                            // ảnh đại diện
                            fileName = "p-" + idmax.ToString() + ext;
                            sp.HinhURL = fileName;
                            fname = Path.Combine(Server.MapPath("~/Content/images/products/"), fileName);
                            file.SaveAs(fname);
                            db.SanPhams.Add(sp);
                        }
                        else
                        {
                            idmax2 = idmax2 + 1;
                            FileInfo fi = new FileInfo(file.FileName);
                            string ext = fi.Extension;
                            // Upload vào table HinhAnhSanPham
                            fileName = "slide-p-" + idmax.ToString() + "-" + idmax2.ToString() + ext;
                            HinhAnhSanPham hssp = new HinhAnhSanPham();
                            hssp.MaSanPham = sp.MaSanPham;
                            hssp.HinhURL = fileName;
                            db.HinhAnhSanPhams.Add(hssp);
                            fname = Path.Combine(Server.MapPath("~/Content/images/products/"), fileName);
                            file.SaveAs(fname);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return "error_images";
                }
            }
            else
            {
                return "error_images";
            }
            if (db.SaveChanges() > 0)
            {
                return "success";
            }
            else
            {
                return "Lỗi sản phẩm";
            }
        }
        [HttpPost]
        public string suasanpham(FormCollection fm)
        {
            int id = Convert.ToInt32(fm["masanpham"]);
            SanPham sp = db.SanPhams.SingleOrDefault(s => s.MaSanPham == id);
            if (sp == null)
            {
                return "Không tồn tại sản phẩm này";
            }
            sp.TenSanPham = Convert.ToString(fm["tensanpham"].Trim());
            sp.SoLuongTon = Convert.ToInt32(fm["soluong"]);
            sp.GiaSanPham = Convert.ToDecimal(fm["giaban"]);
            sp.MaLoaiSanPham = Convert.ToInt32(fm["MaLoaiSanPham"]);
            sp.MaHangSanXuat = Convert.ToInt32(fm["MaHangSanXuat"]);
            sp.MoTa = Convert.ToString(fm["mota"].Trim());


            HttpPostedFileBase hinhdaidien = Request.Files["image"];
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file_check = files[1];
            // check có file mới để upload hay không
            if (file_check.FileName != "" || hinhdaidien.FileName != "")
            {
                // Check nếu có file upload
                string fname;
                int idmax = 1;
                int idmax2 = 0;
                if (db.HinhAnhSanPhams.Count() > 0)
                    idmax2 = db.HinhAnhSanPhams.OrderByDescending(u => u.MaHinh).FirstOrDefault().MaHinh;

                if (db.SanPhams.Count() > 0)
                {
                    idmax = db.SanPhams.OrderByDescending(u => u.MaSanPham).FirstOrDefault().MaSanPham;
                }
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string fileName;// lưu rename tên file
                    // nếu có thay đổi hình đại diện: xóa hình cũ và upload hinh mới(replace ảnh đại diện)
                    if (file.FileName == hinhdaidien.FileName && file.FileName != "")
                    {
                        // lưu hình đại diện
                        FileInfo fi = new FileInfo(file.FileName);
                        string ext = fi.Extension;
                        // ảnh đại diện
                        fname = Path.Combine(Server.MapPath("~/Content/images/products/"), sp.HinhURL);
                        file.SaveAs(fname);
                    }
                    else
                    {
                        if (file.FileName != "")
                        {
                            // upload hình slide thêm vào
                            idmax2 = idmax2 + 1;
                            FileInfo fi = new FileInfo(file.FileName);
                            string ext = fi.Extension;
                            // Upload vào table HinhAnhSanPham
                            fileName = "slide-p-" + idmax.ToString() + "-" + idmax2.ToString() + ext;
                            HinhAnhSanPham hssp = new HinhAnhSanPham();
                            hssp.MaSanPham = sp.MaSanPham;
                            hssp.HinhURL = fileName;
                            db.HinhAnhSanPhams.Add(hssp);
                            fname = Path.Combine(Server.MapPath("~/Content/images/products/"), fileName);
                            file.SaveAs(fname);
                        }
                    }
                }
            }
            db.SaveChanges();
            return "success";
        }
        // Ajax show popover detail product
        [HttpPost]
        public string DetailProduct(int id)
        {
            SanPham sp = db.SanPhams.Single(s => s.MaSanPham == id);
            string img = "<img src='../Content/images/products/" + sp.HinhURL + "' class='img-circle img-sm' />";
            string out1 = img + "<div class='row'>" +
                                "<p>" +
                                    "<label>Tên:&nbsp;</label>" +
                                    "<strong>" + sp.TenSanPham + "</strong>" +
                                "</p>" +
                                 "<p>" +
                                    "<label>Giá:&nbsp;</label>" +
                                    "<strong>" + String.Format("{0:0,0}", sp.GiaSanPham) + "đ</strong>" +
                                "</p>" +
                                 "<p>" +
                                    "<label>Thuộc loại:&nbsp;</label>" +
                                    "<strong>" + sp.LoaiSanPham.TenLoaiSanPham + "</strong>" +
                                "</p>" +
                                "<p>" +
                                    "<label>Đối tác:&nbsp;</label>" +
                                    "<strong>" + sp.HangSanXuat.TenHangSanXuat + "</strong>" +
                                "</p>" +
                                "<p>" +
                                    "<label>Số lượng tồn:&nbsp;</label>" +
                                    "<strong>" + sp.SoLuongTon + "</strong>" +
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

        public ActionResult UpdateProduct(int id)
        {
            SanPham sp = db.SanPhams.SingleOrDefault(s => s.MaSanPham == id);

            if (sp == null)
            {
                RedirectToAction("Index");
            }
            List<HinhAnhSanPham> hsp = db.HinhAnhSanPhams.Where(h => h.MaSanPham == id).ToList();

            ViewData["ListImagesPro"] = hsp;
            ViewData["SanPham"] = sp;
            List<LoaiSanPham> lsp = db.LoaiSanPhams.ToList<LoaiSanPham>();
            var selectedValue = sp.MaLoaiSanPham;
            ViewBag.LoaiSanPhams = new SelectList(lsp, "MaLoaiSanPham", "TenLoaiSanPham", selectedValue);
            var selectedValue1 = sp.MaHangSanXuat;
            List<HangSanXuat> hsx = db.HangSanXuats.ToList<HangSanXuat>();
            ViewBag.HangSanXuats = new SelectList(hsx, "MaHangSanXuat", "TenHangSanXuat", selectedValue1);
            return View();
        }

        [HttpPost]
        public string DeleteImages(string id)
        {
            // Xóa ảnh con
            int idhinh = Convert.ToInt32(id);
            HinhAnhSanPham hsp = db.HinhAnhSanPhams.Where(s => s.MaHinh == idhinh).Single();
            string path = Path.Combine(Server.MapPath("~/Content/images/products/"), hsp.HinhURL);
            if ((System.IO.File.Exists(path)))
            {
                System.IO.File.Delete(path);
            }
            else
            {
                return "error";
            }
            db.HinhAnhSanPhams.Remove(hsp);
            if (db.SaveChanges() > 0)
            {
                return "sucess";
            }
            else
            {
                return "error";
            }

        }


    }
}