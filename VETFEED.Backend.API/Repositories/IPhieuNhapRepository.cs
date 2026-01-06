using VETFEED.Backend.API.DTOs.PhieuNhap;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public interface IPhieuNhapRepository
    {
        Task<IEnumerable<PhieuNhapResponse>> GetAllPhieuNhapsAsync();
        Task<PhieuNhapDetailedResponse?> GetPhieuNhapByIdAsync(Guid id);
        Task<PhieuNhapResponse> AddPhieuNhapAsync(PhieuNhapRequest request);
        Task<PhieuNhapResponse?> UpdatePhieuNhapAsync(Guid id, PhieuNhapRequest request);
        Task<bool> DeletePhieuNhapAsync(Guid id);

        // Lay entity PhieuNhap (khong phai DTO) de dung trong service
        Task<PhieuNhap?> GetPhieuNhapEntityByIdAsync(Guid id);

        // Cap nhat ThanhTien va TrangThai cho PhieuNhap
        Task<bool> UpdatePhieuNhapThanhTienAndTrangThaiAsync(Guid id, decimal thanhTien, TrangThaiPhieuNhapEnum trangThai);
    }
}
