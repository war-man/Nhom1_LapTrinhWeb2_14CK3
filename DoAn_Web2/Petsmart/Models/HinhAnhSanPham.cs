//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Petsmart.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class HinhAnhSanPham
    {
        public int MaHinh { get; set; }
        public int MaSanPham { get; set; }
        public string HinhURL { get; set; }
    
        public virtual SanPham SanPham { get; set; }
    }
}
