using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
using System.Text.RegularExpressions;
using PagedList;

namespace Petsmart.Controllers
{

    public class AccountController : Controller
    {
        private ShopBanDongVatEntities db = new ShopBanDongVatEntities();
        // GET: Account
        public ActionResult Index()
        {
            if (Session["user"] != null)
            {
                var tk = Session["user"] as TaiKhoan;
                ViewBag.TenTaiKhoan = tk.TenHienThi;
                TaiKhoan tka = db.TaiKhoans.SingleOrDefault(t => t.MaTaiKhoan == tk.MaTaiKhoan);
                if (tka == null)
                {
                    return RedirectToAction("Index", "Home");

                }
                else ViewData["TaiKhoan"] = tka;
                List<DonDatHang> lstDonHang = db.DonDatHangs.Where(lst => lst.MaTaiKhoan == tk.MaTaiKhoan).ToList();
                if (lstDonHang != null)
                    ViewData["ListDonHang"] = lstDonHang;
                else ViewData["ListDonHang"] = null;
            }
            else
                return RedirectToAction("Index", "Home");

            return View();
        }
        public string thaydoithongtin(FormCollection fm)
        {
            string Email = fm["Email"].Trim();
            int mataikhoan = Convert.ToInt32(fm["mataikhoan"]);
            bool isEmail = Regex.IsMatch(Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            string SoDienThoai = fm["SoDienThoai"].Trim();
            bool isPhone = Regex.IsMatch(SoDienThoai, @"^(0\d{9,10})$", RegexOptions.IgnoreCase);
            if (isEmail != true)
            {
                return "Email không hợp lệ";
            }
            if (isPhone != true) return "Số điện thoại không hợp lệ";
            string HovaTen = fm["HovaTen"].Trim();
            string TenDangNhap = fm["TenDangNhap"].Trim();
            string DiaChi = fm["DiaChi"].Trim();

            if (DiaChi.Length <= 0 || Email.Length <= 0 || HovaTen.Length <= 0 || TenDangNhap.Length <= 0
                || SoDienThoai.Length <= 0) return "Chưa điền đầy đủ thông tin";
            //sau khi check

            var tk = db.TaiKhoans.Where(e => e.MaTaiKhoan == mataikhoan).SingleOrDefault();
            if (tk.Email != Email)
            {
                var check = db.TaiKhoans.Where(t => t.Email == Email).FirstOrDefault();
                if (check !=null) return "Email đã tồn tại";
            }
            tk.DiaChi = DiaChi;
            tk.Email = Email;
            tk.TenHienThi = HovaTen;
            tk.TenDangNhap = TenDangNhap;
            tk.DienThoai = SoDienThoai;
            db.SaveChanges();
            return "success";
            
        }
        public ActionResult ViewAllInvoices(string currentFilter, string searchString, int? page)
        {
            if (Session["user"] != null)
            {
                var tk = Session["user"] as TaiKhoan;
                ViewBag.TenTaiKhoan = tk.TenHienThi;
                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }
                ViewBag.CurrentFilter = searchString;

                var donhangs = from s in db.DonDatHangs
                                select s;
                donhangs = donhangs.Where(s => s.MaTaiKhoan == tk.MaTaiKhoan).OrderByDescending(a=>a.NgayLap);
                if (!String.IsNullOrEmpty(searchString))
                {
                    donhangs = donhangs.Where(s => s.MaDonDatHang.Contains(searchString));
                }
                int pageSize = 2;
                int pageNumber = (page ?? 1);
                return View(donhangs.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
           
        }
        public ActionResult DetailOrder()
        {
            if (Session["user"] != null)
            {
                string mdh = this.Request.QueryString["mdh"];
                if(mdh == "")
                {
                    return RedirectToAction("Index");
                }
                DonDatHang dh = new DonDatHang();
                dh = db.DonDatHangs.SingleOrDefault(d => d.MaDonDatHang == mdh);
                if (dh == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["DonHang"] = dh;
                    List<ChiTietDonHang> ctdh = new List<ChiTietDonHang>();
                    ctdh = db.ChiTietDonHangs.Where(ct => ct.MaDonDatHang == mdh).ToList();
                    ViewData["LstChiTietDonHang"] = ctdh;
                    
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

    }
}