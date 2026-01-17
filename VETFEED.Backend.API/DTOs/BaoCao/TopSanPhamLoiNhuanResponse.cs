namespace VETFEED.Backend.API.DTOs.BaoCao
{
    // Thông tin lợi nhuận của từng sản phẩm
    public class TopSanPhamLoiNhuanResponse
    {
        public string? TenSanPham { get; set; }
        public decimal DoanhThu { get; set; }
        public decimal ChiPhi { get; set; }
        public decimal LoiNhuan { get; set; }
    }
}
