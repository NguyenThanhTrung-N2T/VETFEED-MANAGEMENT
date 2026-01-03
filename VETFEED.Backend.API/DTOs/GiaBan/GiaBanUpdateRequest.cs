using System;
using System.ComponentModel.DataAnnotations;

namespace VETFEED.Backend.API.DTOs.GiaBan
{
    public class GiaBanUpdateRequest
    {
        [Range(0, double.MaxValue)]
        public decimal DonGiaBan { get; set; }

        [Required]
        public DateTime TuNgay { get; set; }

        public DateTime? DenNgay { get; set; }
        public string? GhiChu { get; set; }
    }
}
