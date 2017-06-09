using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
using PagedList;
using System.IO;
using System.Data.Entity.Validation;

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
            string mdh = this.Request.QueryString["mdh"];
            DonDatHang dh = new DonDatHang();
            dh = db.DonDatHangs.SingleOrDefault(d => d.MaDonDatHang == mdh);
            
            if (dh == null)
            {
                RedirectToAction("Index");
            }
            else
            {
                ViewData["DonHang"] = dh;
                List<ChiTietDonHang> ctdh = new List<ChiTietDonHang>();
                ctdh = db.ChiTietDonHangs.Where(ct => ct.MaDonDatHang == mdh).ToList();
                ViewData["LstChiTietDonHang"] = ctdh;
            }
            return View();
        }
        public string UpdateTinhTrang(string id)
        {
            string[] words = id.Split(',');

            string madonhang = words[0];
            int matinhtrang = Convert.ToInt32(words[1]);

            DonDatHang dh = db.DonDatHangs.SingleOrDefault(d => d.MaDonDatHang == madonhang);

            if(dh == null)
            {
                return "Không tồn tại đơn hàng";
            }
            // cập nhật lại số lượng sản phẩm khi update tình trạng.
            List<ChiTietDonHang> ctdh = db.ChiTietDonHangs.Where(ct => ct.MaDonDatHang == madonhang).ToList();
            // ma tinh trang cu khac dang giao thi tru slsp trong dh va + so luong ban
            if(dh.MaTinhTrang != 2 && (matinhtrang == 2 || matinhtrang == 3))
            {
                foreach(var item in ctdh)
                {
                    // Tìm sp thuộc ctdh
                    SanPham sp = db.SanPhams.Single(s=> s.MaSanPham == item.MaSanPham);
                    // lấy số lượng tồn trừ cho số lượng sp trong đơn hàng
                    sp.SoLuongTon = sp.SoLuongTon - Convert.ToInt32(item.SoLuong);
                    // + số lượng bán
                    sp.SoLuongBan += Convert.ToInt32(item.SoLuong);
                    db.SaveChanges(); 
                }
            }
            // tình trạng cũ là đang giao và tình trạng mới là  đã hủy thì cộng lại sl sp trong ĐH
            else if (dh.MaTinhTrang == 2 && matinhtrang == 4) 
            {
                foreach (var item in ctdh)
                {
                    SanPham sp = db.SanPhams.Single(s => s.MaSanPham == item.MaSanPham);
                    // lấy số lượng tồn + cho số lượng sp trong ct đơn hàng
                    sp.SoLuongTon = sp.SoLuongTon + Convert.ToInt32(item.SoLuong);
                    db.SaveChanges();
                }
            }
            // cập nhật lại tình trạng
            dh.MaTinhTrang = matinhtrang;
            db.SaveChanges();
                return "success";
          
        }
    }
}