using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
using System.Text.RegularExpressions;
using PagedList;
using System.Security.Cryptography;
using System.Text;

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
        public ActionResult DoiMatKhau()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var tk = Session["user"] as TaiKhoan;
            ViewBag.TenTaiKhoan = tk.TenHienThi;
            return View();
        }
        public string Fromdoimatkhau(FormCollection fm)
        {
            string MatKhauCu = fm["MatKhauCu"].Trim();
            string md5mkc = EncodeMD5(MatKhauCu);
            var tk = Session["user"] as TaiKhoan;
            ViewBag.TenTaiKhoan = tk.TenHienThi;
            if (tk.MatKhau.Equals( md5mkc) == false)
            {
                return "Mật khẩu cũ không đúng!";
            }
            else
            {
                // cập nhật mật khẩu mới
                string MatKhauMoi = fm["MatKhauMoi"].Trim();
                string XacNhanMatKhauMoi = fm["XacNhanMatKhauMoi"].Trim();

                if(MatKhauMoi.Equals(XacNhanMatKhauMoi) == false)
                {
                    return "Mật khẩu xác nhận không đúng!";
                }
                string md5mkm = EncodeMD5(MatKhauMoi);
                TaiKhoan tk1 = db.TaiKhoans.SingleOrDefault(s => s.MaTaiKhoan == tk.MaTaiKhoan);
                tk1.MatKhau = md5mkm;
                db.SaveChanges();
                return "success";
            }

        }
        public ActionResult ResetPass()
        {
            return View();
        }
        private string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        public string formResetPass(FormCollection fm)
        {
            string email = fm["Email"].Trim();

            TaiKhoan tk = db.TaiKhoans.SingleOrDefault(t => t.Email.Equals(email));

            if(tk == null)
            {
                return "Email không tồn tại";
            }
            else
            {
                // random mã xác nhận gửi qua email
                // lưu session email để cập nhật mật khẩu
                Session["EmailUser"] = email;
                string randomkeyemail = RandomString(10, true);
                new MailHelper().SendMail(email, "WebPetsmart - Lấy lại mật khẩu ", "Mã xác thực của bạn là: "+randomkeyemail);
                Session["KeyEmail"] = randomkeyemail;
                // gửi mã xác nhận cho user
                return "success";
            }

        }
        public ActionResult KeyEmail()
        {
            if(Session["EmailUser"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public string checkKeyEmail(FormCollection fm)
        {
            string KeyEmail = fm["KeyEmail"].Trim();
            string sessionKeyEmail = Session["KeyEmail"] as string;
            if (KeyEmail.Equals(sessionKeyEmail) == true)
            {
                return "success";
            }
            else return "Mã xác thực không đúng";
        }
        public ActionResult NewPass()
        {
            if (Session["EmailUser"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public string checkNewPass(FormCollection fm)
        {
            string PassNew = fm["PassNew"].Trim();
            string EmailUser = Session["EmailUser"] as string;
            TaiKhoan tk = db.TaiKhoans.SingleOrDefault(t => t.Email.Equals(EmailUser));

            if (tk == null)
            {
                return "Email không tồn tại";
            }
            else
            {
                // reset lại session
                Session["EmailUser"] = null;
                Session["KeyEmail"] = null;
                // Cập nhật mật khẩu
                string md5mkm = EncodeMD5(PassNew);
                tk.MatKhau = md5mkm;
                db.SaveChanges();
                return "success";
            }

        }


    }
}