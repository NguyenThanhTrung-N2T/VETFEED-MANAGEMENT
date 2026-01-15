using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.DTOs.CongNo;
using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public interface ICongNoRepository
    {
        // lay cong no tong hop cua tat ca doi tuong
        Task<List<CongNoTongHopResponse>> GetTongHopCongNoAsync();

        // lay lịch su cong no cua doi tuong theo ma doi tuong
        Task<List<CongNoHistoryRawDto>> GetCongNoHistoryAsync(Guid maDoiTuong);

        // lay khach hang 
        Task<KhachHang?> GetKhachHangByIdAsync(Guid maKH);

        // lay nha cung cap
        Task<NhaCungCap?> GetNhaCungCapByIdAsync(Guid maNCC);

        // lya phieu ban theo ma phieu ban code
        Task<PhieuBan?> GetPhieuBanByCodeAsync(string maPBCode);

        // lay phieu nhap theo ma phieu nhap code
        Task<PhieuNhap?> GetPhieuNhapByCodeAsync(string maPNCode);

        // tinh tong cong no theo phieu
        Task<decimal> GetTongCongNoTheoPhieuAsync(Guid maPhieu);

        // lay danh sach cong no chua tat toan cua doi tuong
        Task<List<CongNo>> GetCongNoChuaTatToanAsync(Guid maDoiTuong);

        // them cong no moi
        Task AddCongNoAsync(CongNo congNo);

        // luu thay doi
        Task SaveChangesAsync();

    }
}
