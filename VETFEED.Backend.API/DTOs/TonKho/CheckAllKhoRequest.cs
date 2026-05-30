using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.DTOs.TonKho
{
    public class CheckAllKhoRequest
    {
        [Required(ErrorMessage = "Mã lô không được để trống !")]
        public Guid MaLo { get; set; }

        [Required(ErrorMessage = "Số lượng cần không được để trống !")]
        [Range(0, double.MaxValue, ErrorMessage = "Số lượng cần không được là số âm !")]
        public decimal SoLuongCan { get; set; }
    }
}
