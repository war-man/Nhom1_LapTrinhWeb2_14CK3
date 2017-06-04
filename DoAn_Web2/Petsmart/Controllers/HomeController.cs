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
using Facebook;
using System.Configuration;

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

        public ActionResult Index()
        {
            ViewData["lstDanhMuc"] = db.SanPhams.OrderByDescending(s => s.LuotXem).Where(s => s.MaLoaiSanPham < 5).Select(s => s).ToList();
            ViewData["lstSanPham"] = db.SanPhams.OrderByDescending(s => s.NgayNhap).Select(s => s).Take(6).ToList();
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
                    // user
                    if (tk != null && tk.MaLoaiTaiKhoan == 1)
                    {
                        Session["user"] = tk;
                        return RedirectToAction("Index", "Account");
                    }
                    // admin
                    else if(tk!=null && tk.MaLoaiTaiKhoan == 2)
                    {
                        Session["admin"] = tk;
                        return RedirectToAction("Index", "AHome");
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
        // Login Facebook
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });
            return Redirect(loginUrl.AbsoluteUri);
        }
        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });
            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                // Get the user's information, like email, first name, middle name etc
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string email = me.email;
                string userName = me.email;
                string firstname = me.first_name;
                string middlename = me.middle_name;
                string lastname = me.last_name;
                // If đã tồn tại email thì set session User
                TaiKhoan tk = db.TaiKhoans.Where(t=>t.BiXoa == false).SingleOrDefault(t => t.Email.Equals(email));
                if(tk != null)
                {
                    Session["user"] = tk;
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    TaiKhoan tkface = new TaiKhoan();
                    // Thêm vào table Tài Khoản
                    tkface.Email = email;
                    tkface.TenDangNhap = email;
                    tkface.TenHienThi = firstname+ " " + middlename+ " " + lastname;
                    tkface.MaLoaiTaiKhoan = 1;
                    tkface.BiXoa = false;
                    tkface.MatKhau = EncodeMD5("123456");
                    tkface.DiaChi = "Mật khẩu mặc định của bạn là:(123456) - Bạn có thể đổi mật khẩu";
                    tkface.DienThoai = "";
                    db.TaiKhoans.Add(tkface);
                    db.SaveChanges();
                    Session["user"] = tkface;
                    return RedirectToAction("Index", "Account");
                }

            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Error()
        {
            return View();
        }
    }
}