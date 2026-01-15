using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.CongNo
{
    public class CreateCongNoRequest
    {
        [Required(ErrorMessage = "Mã đối tượng không được để trống")]
        public Guid MaDoiTuong { get; set; } // mã đối tượng 

        [Required(ErrorMessage = "Số tiền không được để trống")]
        public decimal SoTien { get; set; }
        // > 0 : tăng công nợ
        // < 0 : giảm công nợ

        [Required(ErrorMessage = "Ngày phát sinh không được để trống")]
        public DateTime NgayPhatSinh { get; set; }

        // TÙY CHỌN
        public string? MaPhieuCode { get; set; }
        public DateTime? HanThanhToan { get; set; } // chỉ dùng khi tăng nợ
        public string? GhiChu { get; set; }
    }

}
