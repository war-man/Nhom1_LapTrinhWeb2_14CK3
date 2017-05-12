using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Petsmart.Models;
namespace Petsmart.Controllers
{
    public class ItemCart
    {
        private SanPham sp = new SanPham();
        private int soluong;

        public SanPham Sp
        {
            get
            {
                return sp;
            }

            set
            {
                sp = value;
            }
        }

        public int Soluong
        {
            get
            {

                return soluong;
            }

            set
            {
                soluong = value;
            }
        }
        public ItemCart(SanPham sp ,int soluong)
        {
            this.Sp = sp;
            this.Soluong = soluong;
        }
    }
}