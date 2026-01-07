using VETFEED.Backend.API.DTOs.CTPhieuNhap;

namespace VETFEED.Backend.API.DTOs.PhieuNhap
{
    /// <summary>
    /// Response chi tiết PhieuNhap, bao gồm danh sách chi tiết phiếu nhập
    /// </summary>
    public class PhieuNhapDetailedResponse
    {
        public Guid MaPN { get; set; }
        public string? MaPNCode { get; set; }
        public Guid MaNCC { get; set; }
        public string? TenNCC { get; set; }
        public Guid MaKho { get; set; }
        public string? TenKho { get; set; }
        public decimal ThanhTien { get; set; }
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public List<CTPhieuNhapResponse>? DanhSachChiTiet { get; set; }
    }
}
