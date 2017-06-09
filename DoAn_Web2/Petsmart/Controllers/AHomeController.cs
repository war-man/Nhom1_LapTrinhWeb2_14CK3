using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Petsmart.Controllers
{
    public class AHomeController : Controller
    {
        private ShopBanDongVatEntities db = new ShopBanDongVatEntities();
        // GET: AHome
        public ActionResult Index()
        {
            
            // top 10 khách hàng tổng tiền mua nhiều hàng nhất
            List<Top10User> lstTop10 = new List<Top10User>();
            var query = (from dh in db.DonDatHangs
                         group dh by dh.MaTaiKhoan into gr
                         select new
                         {
                             matk = gr.Key,
                             tongtien = gr.Sum(x => x.TongThanhTien)
                         }).OrderByDescending(i => i.tongtien).Take(10);
            foreach (var item in query)
            {
                Top10User t = new Top10User();
                TaiKhoan tk = db.TaiKhoans.Single(s => s.MaTaiKhoan == item.matk);
                t.Matk = tk.MaTaiKhoan;
                t.Tenkhach = tk.TenHienThi;
                t.Tongtien = item.tongtien;
                t.Sdt = tk.DienThoai;
                lstTop10.Add(t);
            }
            ViewData["lstTop10"] = lstTop10;
            // Top 10 sản phẩm xem nhiều
            var top10sp = db.SanPhams.OrderByDescending(s => s.LuotXem).Take(10).ToList();
            ViewData["top10sp"] = top10sp;

            // Top 10 sản phẩm bán chạy
            var top10spbc = db.SanPhams.OrderByDescending(s => s.SoLuongBan).Take(10).ToList();
            ViewData["top10spbc"] = top10spbc;

            //Số lượng User
            var sumUser = db.TaiKhoans.Where(s => s.MaLoaiTaiKhoan == 1).Count();
            ViewBag.SoLuongUser = sumUser;
            var sumInvoice = db.DonDatHangs.Count();
            ViewBag.SoLuongDonHang = sumInvoice;
            var soluongSPBan = db.ChiTietDonHangs.Sum(s => s.SoLuong);
            ViewBag.SoLuongSPBan = soluongSPBan;
            // get cbb năm có trong đơn hàng Distinct năm
            var ddh = db.DonDatHangs.OrderByDescending(s=>s.NgayLap.Year).Select(s=>s.NgayLap.Year).Distinct();
            List<int> lstyear = new List<int>();
            foreach (var item in ddh)
            {
               lstyear.Add(item);
            }
            ViewData["yearDDH"] = lstyear; 
            return View();
        }
        public JsonResult thongKeDoanhThu(int id)
        {
            // 1: Loại Sản Phẩm 2: Hãng Sản Xuất 3: Sản phẩm
            if (id == 1)
            {
                var mix = (from lsp in db.LoaiSanPhams.DefaultIfEmpty()
                           join sp in db.SanPhams on lsp.MaLoaiSanPham equals sp.MaLoaiSanPham
                           join ctddh in db.ChiTietDonHangs on sp.MaSanPham equals ctddh.MaSanPham
                           where ctddh.DonDatHang.MaTinhTrang == 3
                           select new { MaLoai = lsp.MaLoaiSanPham, TenLoaiSP = lsp.TenLoaiSanPham, Gia = ctddh.GiaBan* ctddh.SoLuong }
                    ).ToList();
                var dataChart = (
                                    from rst in mix
                                    group rst by new { rst.MaLoai, rst.TenLoaiSP } into gr
                                    select new ThongKeDoanhThu { Label = gr.Key.TenLoaiSP, DoanhThu = Convert.ToDecimal(gr.Sum(dt => dt.Gia)) }
                                 ).ToList();
                return Json(dataChart, JsonRequestBehavior.AllowGet);
            }
            // Hãng sản xuất
            else if(id == 2)
            {
                var mix = (from lsp in db.HangSanXuats.DefaultIfEmpty()
                           join sp in db.SanPhams on lsp.MaHangSanXuat equals sp.MaHangSanXuat
                           join ctddh in db.ChiTietDonHangs on sp.MaSanPham equals ctddh.MaSanPham
                           where ctddh.DonDatHang.MaTinhTrang == 3
                           select new { MaLoai = lsp.MaHangSanXuat, TenLoaiSP = lsp.TenHangSanXuat, Gia = ctddh.GiaBan * ctddh.SoLuong }
                    ).ToList();
                var dataChart = (
                                    from rst in mix
                                    group rst by new { rst.MaLoai, rst.TenLoaiSP } into gr
                                    select new ThongKeDoanhThu { Label = gr.Key.TenLoaiSP, DoanhThu = Convert.ToDecimal(gr.Sum(dt => dt.Gia)) }
                                 ).ToList();
                return Json(dataChart, JsonRequestBehavior.AllowGet);
            }
            // Sản phẩm
            else if(id == 3)
            {
                var mix = (from sp in db.SanPhams
                           join ctddh in db.ChiTietDonHangs on sp.MaSanPham equals ctddh.MaSanPham
                           where ctddh.DonDatHang.MaTinhTrang == 3
                           select new { MaLoai = sp.MaSanPham, TenLoaiSP = sp.TenSanPham, Gia = ctddh.GiaBan * ctddh.SoLuong }
                    ).ToList();
                var dataChart =(
                                    from rst in mix
                                    group rst by new { rst.MaLoai, rst.TenLoaiSP } into gr
                                    select new ThongKeDoanhThu { Label = gr.Key.TenLoaiSP, DoanhThu = Convert.ToDecimal(gr.Sum(dt => dt.Gia)) }
                                 ).ToList();
                return Json(dataChart, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var mix = (from sp in db.SanPhams
                           join ctddh in db.ChiTietDonHangs on sp.MaSanPham equals ctddh.MaSanPham
                           where ctddh.DonDatHang.MaTinhTrang == 3
                           select new { MaLoai = sp.MaSanPham, TenLoaiSP = sp.TenSanPham, Gia = ctddh.GiaBan * ctddh.SoLuong }
                    ).ToList();
                var dataChart = (
                                    from rst in mix
                                    group rst by new { rst.MaLoai, rst.TenLoaiSP } into gr
                                    select new ThongKeDoanhThu { Label = gr.Key.TenLoaiSP, DoanhThu = Convert.ToDecimal(gr.Sum(dt => dt.Gia)) }
                                 ).ToList();
                return Json(dataChart, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult thongkeTheoThang(int id)
        {
            // Chọn năm thống kê tháng theo năm đã chọn
            // 1. Lấy danh sách đơn hàng trong năm đã chọn
            // 2. Tính tổng thành tiền trong đơn hàng theo từng tháng
            // 2.1 Group by theo tháng , tính tổng tiền trong tháng
            var mix = (from ddh in db.DonDatHangs
                       join ctddh in db.ChiTietDonHangs on ddh.MaDonDatHang equals ctddh.MaDonDatHang
                       where ddh.MaTinhTrang == 3 && ddh.NgayLap.Year == id
                       select new { NgayLap = ddh.NgayLap, Gia = ctddh.GiaBan/1000000 * ctddh.SoLuong }
                    ).ToList();
            var ma = mix.OrderBy(s => s.NgayLap.Month);
            var dataChart = (
                                   from rst in ma
                                   group rst by new { rst.NgayLap.Month} into gr
                                   select new ThongKeDoanhThu { Label = gr.Key.Month.ToString(), DoanhThu = Convert.ToDecimal(gr.Sum(dt => dt.Gia)) }
                                ).ToList();
            return Json(dataChart, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session["admin"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Setting()
        {
            ThongTinShop tt = db.ThongTinShops.SingleOrDefault(t => t.ID == 1);
            if(tt == null)
            {
                return RedirectToAction("Index", "AHome");
            }
            return View(tt);
        }
        public ActionResult ChartTest()
        {
            return View();
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

        public string ResetPassAdmin(FormCollection fm)
        {
            string editor1 = fm["editor1"].ToString();
            string MatKhauCu = fm["MatKhauCu"].Trim();
            string MatKhauMoi = fm["MatKhauMoi"].Trim();
            if (String.IsNullOrEmpty(MatKhauCu) || String.IsNullOrEmpty(MatKhauMoi))
            {
                return "Chưa điền đầy đủ thông tin !";
            }
            string md5mkc = EncodeMD5(MatKhauCu);
            if (Session["admin"] != null)
            {
                TaiKhoan tk = Session["admin"] as TaiKhoan;
                if (tk.MatKhau.Equals(md5mkc) == false)
                {
                    return "Mật khẩu cũ không đúng!";
                }
                else
                {
                    // cập nhật mật khẩu mới
                    string md5mkm = EncodeMD5(MatKhauMoi);
                    TaiKhoan tk1 = db.TaiKhoans.SingleOrDefault(s => s.MaTaiKhoan == tk.MaTaiKhoan);
                    tk1.MatKhau = md5mkm;
                    db.SaveChanges();
                    // Cập nhật lại session
                    Session["admin"] = tk1;
                    return "success";
                }
            }
            return "error";
        }
        [HttpPost, ValidateInput(false)]
        public string UpdateInfoWebsite(FormCollection fm)
        {
            // Lưu thông tin shop
            string gioithieu = fm["gioithieu"].Trim();
            string UrlGoogle = fm["UrlGoogle"].Trim();
            string UrlFacebook = fm["UrlFacebook"].Trim();
            string SoDienThoai = fm["SoDienThoai"].Trim();
            string Email = fm["Email"].Trim();
            if (String.IsNullOrEmpty(gioithieu) || String.IsNullOrEmpty(UrlGoogle) || String.IsNullOrEmpty(UrlFacebook))
            {
               
                return "Thông tin không được rỗng";
            }
            else
            {
                // check email
                bool isEmail = Regex.IsMatch(Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                // check sdt
                bool isPhone = Regex.IsMatch(SoDienThoai, @"^(0\d{9,10})$", RegexOptions.IgnoreCase);
                if (isEmail != true)
                {
                    return "Email không hợp lệ";
                }
                if (isPhone != true) return "Số điện thoại không hợp lệ";

                ThongTinShop tt = db.ThongTinShops.SingleOrDefault(t => t.ID == 1);
                if (tt == null)
                {
                    return "Không tồn tại";
                }
                else
                {
                    // Cập nhật db
                    tt.GoogleURL = UrlGoogle;
                    tt.FacebookURL = UrlFacebook;
                    tt.GioiThieu = gioithieu;
                    tt.SoDienThoai = SoDienThoai;
                    tt.Email = Email;
                    db.SaveChanges();
                    return "success";
                }
            }

        }
       
    }
 }