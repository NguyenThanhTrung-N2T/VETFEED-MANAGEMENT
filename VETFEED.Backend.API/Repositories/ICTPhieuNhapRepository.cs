using VETFEED.Backend.API.DTOs.CTPhieuNhap;
using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public interface ICTPhieuNhapRepository
    {
        Task<IEnumerable<CTPhieuNhapResponse>> GetAllByPhieuNhapAsync(Guid maPN);
        Task<CTPhieuNhapResponse?> GetCTPhieuNhapByIdAsync(Guid id);
        Task<CTPhieuNhap> AddCTPhieuNhapAsync(Guid maPN, Guid maLo, decimal soLuong, decimal donGia = 0);
        Task<bool> DeleteCTPhieuNhapAsync(Guid id);

        // Lay danh sach entities CTPhieuNhap (khong phai DTO) theo MaPN
        Task<IEnumerable<CTPhieuNhap>> GetCTPhieuNhapEntitiesByMaPNAsync(Guid maPN);
    }
}
