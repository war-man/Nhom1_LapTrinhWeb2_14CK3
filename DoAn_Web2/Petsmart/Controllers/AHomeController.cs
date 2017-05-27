using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
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
            //Số lượng User
            var sumUser = db.TaiKhoans.Where(s=>s.MaLoaiTaiKhoan == 1).Count();
            ViewBag.SoLuongUser = sumUser;
            var sumInvoice = db.DonDatHangs.Count();
            ViewBag.SoLuongDonHang = sumInvoice;
            var soluongSPBan = db.ChiTietDonHangs.Sum(s => s.SoLuong);
            ViewBag.SoLuongSPBan = soluongSPBan;
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult ChartTest()
        {
            return View();
        }
    }
}