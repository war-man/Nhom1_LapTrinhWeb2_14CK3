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
    
    public partial class SanPham
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SanPham()
        {
            this.ChiTietDonHangs = new HashSet<ChiTietDonHang>();
            this.HinhAnhSanPhams = new HashSet<HinhAnhSanPham>();
        }
    
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string HinhURL { get; set; }
        public decimal GiaSanPham { get; set; }
        public System.DateTime NgayNhap { get; set; }
        public int SoLuongTon { get; set; }
        public int SoLuongBan { get; set; }
        public int LuotXem { get; set; }
        public string MoTa { get; set; }
        public bool BiXoa { get; set; }
        public int MaLoaiSanPham { get; set; }
        public int MaHangSanXuat { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public virtual HangSanXuat HangSanXuat { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HinhAnhSanPham> HinhAnhSanPhams { get; set; }
        public virtual LoaiSanPham LoaiSanPham { get; set; }
    }
}
