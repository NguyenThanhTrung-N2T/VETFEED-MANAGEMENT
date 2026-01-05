using VETFEED.Backend.API.DTOs.PhieuNhap;

namespace VETFEED.Backend.API.Repositories
{
    public interface IPhieuNhapRepository
    {
        Task<IEnumerable<PhieuNhapResponse>> GetAllPhieuNhapsAsync();
        Task<PhieuNhapDetailedResponse?> GetPhieuNhapByIdAsync(Guid id);
        Task<PhieuNhapResponse> AddPhieuNhapAsync(PhieuNhapRequest request);
        Task<PhieuNhapResponse?> UpdatePhieuNhapAsync(Guid id, PhieuNhapRequest request);
        Task<bool> DeletePhieuNhapAsync(Guid id);
    }
}
