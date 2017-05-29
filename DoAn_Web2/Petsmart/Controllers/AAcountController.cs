using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
using PagedList;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace Petsmart.Controllers
{
    public class AAcountController : Controller
    {
        // GET: AAcount
        private ShopBanDongVatEntities db = new ShopBanDongVatEntities();
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.LoaiSortParm = String.IsNullOrEmpty(sortOrder) ? "loai_desc" : "";
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

            var taikhoans = from s in db.TaiKhoans
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                taikhoans = taikhoans.Where(s => s.TenDangNhap.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    taikhoans = taikhoans.OrderBy(s => s.TenDangNhap);
                    break;
                case "loai_desc":
                    taikhoans = taikhoans.OrderByDescending(s => s.LoaiTaiKhoan.MaLoaiTaiKhoan);
                    break;
                case "id_desc":
                    taikhoans = taikhoans.OrderByDescending(s => s.MaTaiKhoan);
                    break;
                default:
                    taikhoans = taikhoans.OrderByDescending(s => s.MaTaiKhoan);
                    break;
            }
            List<LoaiTaiKhoan> ltk = db.LoaiTaiKhoans.ToList();
            ViewBag.LoaiTaiKhoans = new SelectList(ltk, "MaLoaiTaiKhoan", "TenLoaiTaiKhoan");
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(taikhoans.ToPagedList(pageNumber, pageSize));
        }
        public string LockProduct(int id)
        {       
            
            try
            {
                TaiKhoan tk = db.TaiKhoans.Single(s => s.MaTaiKhoan == id);
                tk.BiXoa = true;
                //tk.XacNhanMK = tk.MatKhau;
                db.SaveChanges();
                return "success";
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }

        }
        public string UnLockProduct(int id)
        {
            try
            {
                TaiKhoan tk = db.TaiKhoans.Single(s => s.MaTaiKhoan == id);
                tk.BiXoa = false;
                //tk.XacNhanMK = tk.MatKhau;
                db.SaveChanges();
                return "success";
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }
        }
        public string UpdateQuyen(FormCollection fm)
        {
            int matk = Convert.ToInt32(fm["mataikhoan"]);
            int maltk = Convert.ToInt32(fm["maloaitaikhoan"]);
            try
            {
                TaiKhoan tk = db.TaiKhoans.Single(s => s.MaTaiKhoan == matk);
                tk.MaLoaiTaiKhoan = maltk;
               // tk.XacNhanMK = tk.MatKhau;
                db.SaveChanges();
                return "success";
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }
        }
        public string ChiTietKhachHang(int id)
        {
            TaiKhoan sp = db.TaiKhoans.Where(s=> s.MaLoaiTaiKhoan == 1).Single(s => s.MaTaiKhoan == id);
            string img = "<img src='../Content/admin/img/av3.png' class='img-circle img-sm' />";
            string out1 = img + "<div class='row'>" +
                                "<p>" +
                                    "<label>Tên:&nbsp;</label>" +
                                    "<strong>" + sp.TenHienThi + "</strong>" +
                                "</p>" +
                                 "<p>" +
                                    "<label>Tên đăng nhập:&nbsp;</label>" +
                                    "<strong>" + sp.TenDangNhap + "</strong>" +
                                "</p>" +
                                 "<p>" +
                                    "<label>Email:&nbsp;</label>" +
                                    "<strong>" + sp.Email + "</strong>" +
                                "</p>" +
                                "<p>" +
                                    "<label>Số điện thoại:&nbsp;</label>" +
                                    "<strong>" + sp.DienThoai + "</strong>" +
                                "</p>" +
                                "<p>" +
                                    "<label>Địa chỉ:&nbsp;</label>" +
                                    "<strong>" + sp.DiaChi + "</strong>" +
                                "</p>" +
                        "</div>";
            return out1;
        }


    }
}