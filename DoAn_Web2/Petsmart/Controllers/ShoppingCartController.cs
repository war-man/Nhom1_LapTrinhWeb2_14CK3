using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Petsmart.Models;
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
            string[] quantities = frm.GetValues("quantity");
            List<ItemCart> cart = (List<ItemCart>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
            {
                int quantity = Convert.ToInt32(quantities[i]);
                if (quantity <= 0)
                    cart.RemoveAt(i);

                else
                    cart[i].Soluong = quantity;
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
        public ActionResult CheckOut()
        {
            return RedirectToAction("ViewCart", "ShoppingCart");
        }

        public ActionResult WishList()
        {
            return View();
        }
    }
}