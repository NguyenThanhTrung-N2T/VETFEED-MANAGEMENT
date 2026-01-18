namespace VETFEED.Backend.API.DTOs.BaoCao
{
    // Chi tiết tồn kho từng sản phẩm
    public class TonKhoSanPhamItemResponse
    {
        public Guid MaSP { get; set; }
        public string? MaSPCode { get; set; }
        public string? TenSanPham { get; set; }
        public string? MaLoCode { get; set; }
        public decimal SoLuong { get; set; }
        public string? DonVi { get; set; }
        public DateTime NgayHetHan { get; set; }
        public string? TrangThai { get; set; } // CON_HAN | SAP_HET_HAN | HET_HAN
        public int SoNgayDenKhiHetHan { get; set; }
    }
}
