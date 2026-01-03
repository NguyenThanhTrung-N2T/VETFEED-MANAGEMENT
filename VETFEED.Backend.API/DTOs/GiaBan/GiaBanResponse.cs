using System;

namespace VETFEED.Backend.API.DTOs.GiaBan
{
    public class GiaBanResponse
    {
        public Guid MaGia { get; set; }
        public Guid MaSP { get; set; }
        public string? MaSPCode { get; set; }
        public string? TenSP { get; set; }

        public decimal DonGiaBan { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
