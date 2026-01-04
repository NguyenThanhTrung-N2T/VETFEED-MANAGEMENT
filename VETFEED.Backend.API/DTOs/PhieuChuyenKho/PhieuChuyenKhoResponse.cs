namespace VETFEED.Backend.API.DTOs.PhieuChuyenKho
{
    public class PhieuChuyenKhoResponse
    {
        public Guid MaCK { get; set; }
        public string? MaCKCode { get; set; }
        public DateTime NgayLap { get; set; }
        public string? TenKhoXuat { get; set; }
        public string? TenKhoNhan { get; set; }
        public string? GhiChu { get; set; }
    }

}
