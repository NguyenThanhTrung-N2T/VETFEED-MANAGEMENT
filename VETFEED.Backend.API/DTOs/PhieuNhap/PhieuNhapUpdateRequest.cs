using System.Text.Json.Serialization;
using VETFEED.Backend.API.DTOs.CTPhieuNhap;

namespace VETFEED.Backend.API.DTOs.PhieuNhap
{
    /// <summary>
    /// Request cập nhật phiếu nhập bao gồm:
    /// - Thông tin phiếu nhập (MaNCC, MaKho, TrangThai, GhiChu)
    /// - Danh sách cập nhật chi tiết (DonGia, SoLuong)
    /// </summary>
    public class PhieuNhapUpdateRequest
    {
        public Guid MaNCC { get; set; }
        public Guid MaKho { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public string? TrangThai { get; set; }  // DA_DAT, DA_NHAN, DA_HUY
        
        public string? GhiChu { get; set; }
        
        /// <summary>
        /// Danh sách cập nhật chi tiết phiếu nhập
        /// Mỗi item chứa MaCTPN để xác định dòng, SoLuong và DonGia mới
        /// </summary>
        public List<CTPhieuNhapUpdateRequest>? DanhSachChiTiet { get; set; }
    }
}
