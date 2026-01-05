namespace VETFEED.Backend.API.DTOs.CTPhieuNhap
{
    /// <summary>
    /// Request tạo chi tiết phiếu nhập - Service sẽ tự gán MaPN và tạo LoHang
    /// </summary>
    public class CTPhieuNhapRequest
    {
        // Thông tin lô hàng - Service sẽ tạo LoHang
        public Guid MaSP { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime HanSuDung { get; set; }
        
        public decimal SoLuong { get; set; }
        // DonGia không cần nhập, mặc định = 0
    }
}
