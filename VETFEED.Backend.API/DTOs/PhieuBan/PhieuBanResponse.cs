using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.PhieuBan
{
    public class PhieuBanResponse
    {
        public Guid MaPB { get; set; }
        public string? MaPBCode { get; set; }
        public Guid MaKH { get; set; }
        public string? TenKhachHang { get; set; }
        public DateTime NgayBan { get; set; }
        public decimal TongTienHang { get; set; }
        public decimal ChietKhauPhanTram { get; set; }
        public decimal TienChietKhau { get; set; }
        public decimal ThanhTien { get; set; }
        public string? HinhThucThanhToan { get; set; }
        public string? TrangThaiThanhToan { get; set; }
        public decimal TienCoc { get; set; }
        public decimal TienNo { get; set; }
        public DateTime? HanTra { get; set; }
        public string? GhiChu { get; set; }
        public List<ChiTietPhieuBanResponse>? DanhSachChiTiet { get; set; }
    }

    public class ChiTietPhieuBanResponse
    {
        public Guid MaCTPB { get; set; }
        public Guid MaKho { get; set; }
        public string? TenKho { get; set; }
        public Guid MaLo { get; set; }
        public string? MaLoCode { get; set; }
        public string? TenSanPham { get; set; }
        public string? DonViCoSo { get; set; }
        public decimal SoLuong { get; set; }
        public string? DonViBan { get; set; }
        public decimal DonGia { get; set; }
        public decimal SoLuongQuyDoi { get; set; }
        public decimal GiaVonCoSo { get; set; }
        public decimal ThanhTienVon { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string? GhiChu { get; set; }
    }
}
