using VETFEED.Backend.API.DTOs.CTPhieuNhap;
using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public interface ICTPhieuNhapRepository
    {
        Task<IEnumerable<CTPhieuNhapResponse>> GetAllByPhieuNhapAsync(Guid maPN);
        Task<CTPhieuNhapResponse?> GetCTPhieuNhapByIdAsync(Guid id);
        Task<CTPhieuNhap> AddCTPhieuNhapAsync(Guid maPN, Guid maLo, decimal soLuong, decimal donGia = 0, string? donViNhap = null, decimal soLuongQuyDoi = 0, decimal donGiaCoSo = 0);
        Task<bool> DeleteCTPhieuNhapAsync(Guid id);
        // Lay danh sach entities CTPhieuNhap theo MaPN
        Task<IEnumerable<CTPhieuNhap>> GetCTPhieuNhapEntitiesByMaPNAsync(Guid maPN);
        // Cap nhat SoLuong va DonGia cho CTPhieuNhap
        Task<bool> UpdateCTPhieuNhapAsync(Guid maCTPN, decimal soLuong, decimal donGia, string? donViNhap = null, decimal soLuongQuyDoi = 0, decimal donGiaCoSo = 0);
        // Batch xoa tat ca CTPhieuNhap theo MaPN
        Task<int> DeleteCTPhieuNhapsByMaPNAsync(Guid maPN);
    }
}


