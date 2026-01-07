using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.DTOs.TonKho
{
    public class CheckTonKhoRequest
    {
        [Required(ErrorMessage = "Mã kho xuất không được để trống !")]
        public Guid MaKhoXuat { get; set; }

        [Required(ErrorMessage = "Mã lô không được để trống !")]
        public Guid MaLo { get; set; }

        [Required(ErrorMessage = "Số lượng chuyển không được để trống !")]
        [Range(1,double.MaxValue,ErrorMessage = "Số lượng chuyển phải lớn hơn 0 !")]
        public decimal SoLuongChuyen { get; set; }
    }
}
