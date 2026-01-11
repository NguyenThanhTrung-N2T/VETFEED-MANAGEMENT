namespace VETFEED.Backend.API.DTOs.PhieuBan
{
    public class KhachHangPhieuBanResponse
    {
        // Thông tin khách hàng
        public Guid MaKH { get; set; }
        public string MaKHCode { get; set; } = null!;
        public string TenKH { get; set; } = null!;
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }

        public string? LoaiKhachHang { get; set; }

        public decimal TongMua { get; set; }
        public decimal CongNoHienTai { get; set; }
        public decimal? HanMucCongNo { get; set; }

        //  danh sách phiếu mua của khách hàng
        public List<PhieuBanListResponse> DanhSachPhieuBan { get; set; } = new();
    }

}
