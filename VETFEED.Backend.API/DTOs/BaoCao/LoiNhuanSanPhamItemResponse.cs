namespace VETFEED.Backend.API.DTOs.BaoCao
{
    // Chi tiết lợi nhuận của từng sản phẩm
    public class LoiNhuanSanPhamItemResponse
    {
        public string? MaSPCode { get; set; }
        public string? TenSanPham { get; set; }
        public decimal SoLuongBan { get; set; }
        public decimal DoanhThu { get; set; }
        public decimal ChiPhi { get; set; }
        public decimal LoiNhuan { get; set; }
        public decimal TiSuat { get; set; }
    }
}
