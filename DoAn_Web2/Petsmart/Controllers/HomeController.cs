using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
using PagedList;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

namespace Petsmart.Controllers
{
    public class HomeController : Controller
    {
        private ShopBanDongVatEntities db = new ShopBanDongVatEntities();
        public string EncodeMD5(string pass)

        {

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] bs = System.Text.Encoding.UTF8.GetBytes(pass);

            bs = md5.ComputeHash(bs);

            System.Text.StringBuilder s = new System.Text.StringBuilder();

            foreach (byte b in bs)

            {

                s.Append(b.ToString("x1").ToLower());

            }

            pass = s.ToString();

            return pass;

        }

        public ActionResult Index()
        {
            ViewData["lstDanhMuc"] = db.SanPhams.OrderByDescending(s => s.LuotXem).Where(s => s.MaLoaiSanPham < 5).Select(s => s).ToList();
            ViewData["lstSanPham"] = db.SanPhams.OrderByDescending(s => s.SoLuongBan).Select(s => s).Take(6).ToList();
            ViewData["lstNSX"] = db.HangSanXuats.ToList();

            if (Session["user"] == null)
            {
                RedirectToAction("Index", "Home");
            }

            return View();
        }
        public ActionResult Search(string sortOrder, string currentFilter, string searchString, int? page)
        {
            List<SanPham> sanpham = new List<SanPham>();
            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                sanpham = db.SanPhams.Where(s => s.TenSanPham.Contains(searchString)).ToList();

            }

            ViewData["lstLoaiSanPham"] = db.LoaiSanPhams.ToList();
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(sanpham.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Login()
        {
            if(Session["user"] != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(TaiKhoan t)
        {

            using (ShopBanDongVatEntities db = new ShopBanDongVatEntities())
            {
                string mkmd5 = EncodeMD5(t.MatKhau);
                var tk = db.TaiKhoans.Where(e => (e.Email.Equals(t.Email) || e.TenDangNhap.Equals(t.Email)) && e.MatKhau.Equals(mkmd5) && e.BiXoa.Equals(false)).FirstOrDefault();
                {
                    if (tk != null)
                    {
                        Session["user"] = tk;
                        return RedirectToAction("Index", "Account");
                    }
                }

            }
            return View();
        }

        public ActionResult Logout()
        {
            Session["user"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

        public string dangki(FormCollection fm)
        { 
            // check email
            string Email = fm["Email"].Trim();
            bool isEmail = Regex.IsMatch(Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            // check sdt
            string SoDienThoai = fm["SoDienThoai"].Trim();
            bool isPhone = Regex.IsMatch(SoDienThoai, @"^(0\d{9,10})$", RegexOptions.IgnoreCase);
            if (isEmail != true)
            {
                return "Email không hợp lệ";
            }
            if (isPhone != true) return "Số điện thoại không hợp lệ";
            string MatKhau = fm["MatKhau"].Trim();
            string XacNhanMatKhau = fm["XacNhanMatKhau"].Trim();
            // check trùng xác nhận mật khẩu
            if (MatKhau.Equals(XacNhanMatKhau) == false)
            {
                return "Mật khẩu xác nhận không đúng!";
            }
            string HovaTen = fm["HovaTen"].Trim();
            string TenDangNhap = fm["TenDangNhap"].Trim();
            string DiaChi = fm["DiaChi"].Trim();
            

            // check day du thong tin
            if (DiaChi.Length <= 0 || Email.Length <= 0 || HovaTen.Length <= 0 || TenDangNhap.Length <= 0
                || SoDienThoai.Length <= 0 || MatKhau.Length <=0 || XacNhanMatKhau.Length <=0) return "Chưa điền đầy đủ thông tin";

            // check trùng tài khoản or email
            var tkcheck = db.TaiKhoans.Where(e => (e.Email.Equals(Email) || e.TenDangNhap.Equals(TenDangNhap)) && e.BiXoa.Equals(false)).FirstOrDefault();

            if (tkcheck != null)
            {
                return "Email hoặc tên đăng nhập đã bị trùng !";
            }

            //sau khi check
            TaiKhoan tk = new TaiKhoan();
            tk.MatKhau = EncodeMD5(MatKhau);
            tk.DiaChi = DiaChi;
            tk.Email = Email;
            tk.TenHienThi = HovaTen;
            tk.TenDangNhap = TenDangNhap;
            tk.DienThoai = SoDienThoai;
            // mặc định User
            tk.MaLoaiTaiKhoan = 1;
            db.TaiKhoans.Add(tk);
            if (db.SaveChanges() > 0)
                return "success";
            else
                return "error";

        }
        public ActionResult Error()
        {
            return View();
        }
    }
}