using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Petsmart.Controllers
{
    public class ThongKeDoanhThu
    {
        private string label;
        private decimal doanhThu;

        

        public decimal DoanhThu
        {
            get
            {
                return doanhThu;
            }

            set
            {
                doanhThu = value;
            }
        }

        public string Label
        {
            get
            {
                return label;
            }

            set
            {
                label = value;
            }
        }
    }
}