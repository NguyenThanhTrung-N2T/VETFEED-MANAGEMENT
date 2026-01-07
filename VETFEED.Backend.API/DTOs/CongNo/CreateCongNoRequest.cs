using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.DTOs.CongNo
{
    public class CreateCongNoRequest
    {
        [Required(ErrorMessage = "Loại đối tượng không được để trống !")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LoaiDoiTuongCongNoEnum LoaiDoiTuong { get; set; }

        public Guid MaDoiTuong { get; set; }

    }
}
