using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petsmart.Controllers
{
    public class Top10User
    {
        private int matk;
        private decimal tongtien;
        private string tenkhach;
        private string sdt;

        public int Matk
        {
            get
            {
                return matk;
            }

            set
            {
                matk = value;
            }
        }

        public decimal Tongtien
        {
            get
            {
                return tongtien;
            }

            set
            {
                tongtien = value;
            }
        }

        public string Tenkhach
        {
            get
            {
                return tenkhach;
            }

            set
            {
                tenkhach = value;
            }
        }

        public string Sdt
        {
            get
            {
                return sdt;
            }

            set
            {
                sdt = value;
            }
        }
    }
}