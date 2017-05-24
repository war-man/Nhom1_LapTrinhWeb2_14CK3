using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
using PagedList;
using System.IO;
namespace Petsmart.Controllers
{
    public class AOrdersController : Controller
    {
        private ShopBanDongVatEntities db = new ShopBanDongVatEntities();

        // GET: 
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

            var donhangs = from s in db.DonDatHangs
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                donhangs = donhangs.Where(s => s.TaiKhoan.TenHienThi.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    donhangs = donhangs.OrderBy(s => s.TaiKhoan.TenHienThi);
                    break;
                case "date_desc":
                    donhangs = donhangs.OrderByDescending(s => s.NgayLap);
                    break;
                case "id_desc":
                    donhangs = donhangs.OrderByDescending(s => s.MaDonDatHang);
                    break;
                case "price_desc":
                    donhangs = donhangs.OrderBy(s => s.TongThanhTien);
                    break;
                default:
                    donhangs = donhangs.OrderByDescending(s => s.NgayLap);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(donhangs.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult DetailOrder()
        {
            string mhd = this.Request.QueryString["mhd"];
            DonDatHang dh = new DonDatHang();
            dh = db.DonDatHangs.SingleOrDefault(d => d.MaDonDatHang == mhd);
            if(dh == null)
            {
                RedirectToAction("Index");
            }
            else
            {
                List<ChiTietDonHang> ctdh = new List<ChiTietDonHang>();
                ctdh = db.ChiTietDonHangs.Where(ct => ct.MaDonDatHang == mhd).ToList();
                ViewData["LstChiTietDonHang"] = ctdh;
                ViewData["DonHang"] = dh;
            }
            return View();
        }
    }
}