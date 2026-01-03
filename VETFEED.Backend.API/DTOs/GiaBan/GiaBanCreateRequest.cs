using System;
using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.DTOs.GiaBan
{
    public class GiaBanCreateRequest
    {
        [Required]
        public Guid MaSP { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DonGiaBan { get; set; }

        [Required]
        public DateTime TuNgay { get; set; } // nhập ngày, backend normalize

        public DateTime? DenNgay { get; set; } // nhập ngày, backend normalize
        public string? GhiChu { get; set; }
    }
}
