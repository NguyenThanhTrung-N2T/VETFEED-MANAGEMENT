using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.DTOs.TonKho
{
    public class UpdateTonKhoRequest
    {
        [Range(0, double.MaxValue, ErrorMessage = "Số lượng tồn kho không được là số âm !"),]
        public decimal SoLuong { get; set; }
    }
}
