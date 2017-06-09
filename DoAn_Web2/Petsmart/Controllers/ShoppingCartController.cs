using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
using System.Text.RegularExpressions;

namespace Petsmart.Controllers
{
    public class ShoppingCartController : Controller
    {
        private ShopBanDongVatEntities db = new ShopBanDongVatEntities();
        // GET: ShoppingCart
        public ActionResult Index()
        {
            return View();
        }
        private int KiemTraSanPham(int id)
        {
            List<ItemCart> cart = (List<ItemCart>)Session["cart"];
            for(int i = 0; i< cart.Count; i++)
            {
                if(cart[i].Sp.MaSanPham == id)
                {
                    return i;
                }
            }
            return -1;
        }
       
        public ActionResult ViewCart()
        {
            return View();
        }
        public ActionResult OrderNow(int id)
        {
            // Xử lý nút thêm
            if (Session["cart"] == null)
            {
                List<ItemCart> cart = new List<ItemCart>();
                cart.Add(new ItemCart(db.SanPhams.Find(id), 1));
                Session["cart"] = cart;
            }
            else
            {
                List<ItemCart> cart = (List<ItemCart>)Session["cart"];
                int index = KiemTraSanPham(id);
                if(index == -1)
                {
                    cart.Add(new ItemCart(db.SanPhams.Find(id), 1));
                }
                else
                {
                    cart[index].Soluong++;
                }
                Session["cart"] = cart;
            }
            return RedirectToAction("ViewCart", "ShoppingCart");
        }
        public ActionResult UpdateCart(FormCollection frm)
        {
            if(Session["cart"] == null) return RedirectToAction("ViewCart", "ShoppingCart");

            string[] quantities = frm.GetValues("quantity");
            List<ItemCart> cart = (List<ItemCart>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
            {
                int quantity = Convert.ToInt32(quantities[i]);
                if (quantity <= 0)
                    cart.RemoveAt(i);
                // Số lượng sản phẩm quá số lượng tồn
                else if(cart[i].Soluong > cart[i].Sp.SoLuongTon)
                    return RedirectToAction("ViewCart", "ShoppingCart");
                else cart[i].Soluong = quantity;
            }
            Session["cart"] = cart;
            return RedirectToAction("ViewCart", "ShoppingCart");
        }
        public ActionResult RemoveCart(int id)
        {
            int index = KiemTraSanPham(id);
            List<ItemCart> cart = (List<ItemCart>)Session["cart"];
            if (index != -1)
            {
                cart.RemoveAt(index);
            }
            Session["cart"] = cart;
            return RedirectToAction("ViewCart", "ShoppingCart");
        }
        public ActionResult RemoveAll()
        {
            Session["cart"] = null;
            return RedirectToAction("ViewCart", "ShoppingCart");
        }

        public string CheckOut(FormCollection fm)
        {
            // check có tồn tại user chưa
            if(Session["user"] == null)
            {
                return "error_user";
            }
            // check giỏ hàng null or bằng 0;
            if (Session["cart"] == null)
            {
                return "Chưa có sản phẩm trong giỏ hàng";
                
            }
            else
            {
                // giỏ hàng chưa có sản phẩm
                List<ItemCart> cart = (List<ItemCart>)Session["cart"];
                if (cart.Count < 1)
                {
                    return "Chưa có sản phẩm trong giỏ hàng";

                }
                
                    decimal tongtien = 0;
                    // tính tổng tiền
                    foreach (ItemCart item in cart)
                    {
                        tongtien += item.Sp.GiaSanPham * item.Soluong;
                    }
                // check thông tin giao hàng
                string Email = fm["Email"].Trim();
                TaiKhoan tk = Session["user"] as TaiKhoan;
                bool isEmail = Regex.IsMatch(Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                string SoDienThoai = fm["SoDienThoai"].Trim();
                bool isPhone = Regex.IsMatch(SoDienThoai, @"^(0\d{9,10})$", RegexOptions.IgnoreCase);
                if (isEmail != true)
                {
                    return "Email không hợp lệ";
                }
                if (isPhone != true) return "Số điện thoại không hợp lệ";
                string HovaTen = fm["HovaTen"].Trim();
                string DiaChi = fm["DiaChi"].Trim();
                if (DiaChi.Length <= 0 || Email.Length <= 0 || HovaTen.Length <= 0
                    || SoDienThoai.Length <= 0) return "Chưa điền đầy đủ thông tin";

                // lưu đơn đặt hàng
                DateTime date = DateTime.Now;
                string madonhang ="D" +  date.ToString("yyyyMMddHHmmss");

                DonDatHang ddh = new DonDatHang();
                ddh.MaDonDatHang = madonhang;
                ddh.HoVaTen = HovaTen;
                ddh.MaTaiKhoan = tk.MaTaiKhoan;
                ddh.MaTinhTrang = 1;
                ddh.NgayLap = date;
                ddh.SoDienThoai = SoDienThoai;
                ddh.TongThanhTien = tongtien;
                ddh.DiaChi = DiaChi;
                ddh.Email = Email;
                db.DonDatHangs.Add(ddh);
                // lưu chi tiết đơn hàng
                foreach (ItemCart item in cart)
                {
                    ChiTietDonHang ctdh = new ChiTietDonHang();
                    // mã chi tiết đơn hàng = mã đơn hàng + mã sản phẩm
                    string mactdh = madonhang + item.Sp.MaSanPham;
                    ctdh.MaChiTietDonHang = mactdh;
                    ctdh.MaSanPham = item.Sp.MaSanPham;
                    ctdh.SoLuong = item.Soluong;
                    ctdh.GiaBan = item.Sp.GiaSanPham;
                    ctdh.MaDonDatHang = madonhang;

                    db.ChiTietDonHangs.Add(ctdh);
                }
                if(db.SaveChanges() > 0)
                {
                    // Xóa giỏ hàng
                    Session["cart"] = null;
                    return "success";
                }
                else
                {
                    return "error";
                }
            }

        }

        public ActionResult WishList()
        {
            return View();
        }
    }
}