using VETFEED.Backend.API.DTOs.CTChuyenKho;

namespace VETFEED.Backend.API.DTOs.PhieuChuyenKho
{
    public class ChiTietPhieuChuyenKhoResponse
    {
        public Guid MaCK { get; set; }
        public string? MaCKCode { get; set; }
        public DateTime NgayLap { get; set; }
        public string? TenKhoXuat { get; set; }
        public string? TenKhoNhan { get; set; }
        public string? GhiChu { get; set; }

        // Bổ sung thêm để xử lý logic cập nhật tồn kho
        public Guid MaKhoXuat { get; set; }
        public Guid MaKhoNhan { get; set; }

        public List<CTChuyenKhoResponse>? DanhSachSanPham { get; set; }
    }

}
