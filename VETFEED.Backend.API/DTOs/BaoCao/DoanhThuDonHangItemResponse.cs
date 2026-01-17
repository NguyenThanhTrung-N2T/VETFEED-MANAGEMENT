namespace VETFEED.Backend.API.DTOs.BaoCao
{
    //Chi tiết một dòng đơn hàng trong báo cáo doanh thu
    public class DoanhThuDonHangItemResponse
    {
        public string? MaPhieuBanCode { get; set; }
        public DateTime Ngay { get; set; }
        public string? TenSanPham { get; set; }
        public string? MaSPCode { get; set; }
        public string? TenKhachHang { get; set; }
        public decimal SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}
