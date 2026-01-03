namespace VETFEED.Backend.API.DTOs.KhachHang
{
    public class KhachHangQuery
    {
        public string? Keyword { get; set; }          // MaKHCode | TenKH | SoDienThoai
        public string? LoaiKhachHang { get; set; }    // CA_NHAN | TRANG_TRAI | DAI_LY
        public string? TrangThai { get; set; }        // HOAT_DONG | KHOA
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
