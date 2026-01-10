namespace VETFEED.Backend.API.DTOs.PhieuTra
{
    //Response chi tiết phiếu trả hàng
    public class PhieuTraDetailResponse
    {
        public Guid MaPT { get; set; }
        public string? MaPTCode { get; set; }
        public Guid MaPB { get; set; }
        public string? MaPBCode { get; set; }
        public Guid MaKH { get; set; }
        public string? TenKhachHang { get; set; }
        public DateTime NgayTra { get; set; }
        public decimal ThanhTien { get; set; }
        public string? LyDoTra { get; set; }
        public string? HinhThucHoanTien { get; set; }
        public List<ChiTietPhieuTraDetailResponse>? DanhSachChiTiet { get; set; }
    }

    //Chi tiết phiếu trả hàng
    public class ChiTietPhieuTraDetailResponse
    {
        public Guid MaCTPT { get; set; }
        public Guid MaLo { get; set; }
        public string? MaLoCode { get; set; }
        public string? TenSanPham { get; set; }
        
        public string? DonViCoSo { get; set; }
        
        public string? DonViTra { get; set; }
        
        public decimal SoLuongTra { get; set; }
        
        public decimal DonGiaHoan { get; set; }
        
        public decimal ThanhTienTra { get; set; }
        
        public DateTime? HanSuDung { get; set; }
        
        public string? GhiChu { get; set; }
    }
}
