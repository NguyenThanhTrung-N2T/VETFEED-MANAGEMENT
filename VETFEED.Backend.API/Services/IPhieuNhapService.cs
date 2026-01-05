using VETFEED.Backend.API.DTOs.PhieuNhap;

namespace VETFEED.Backend.API.Services
{
    public interface IPhieuNhapService
    {
        Task<IEnumerable<PhieuNhapResponse>> GetAllPhieuNhapsAsync();
        Task<PhieuNhapDetailedResponse?> GetPhieuNhapByIdAsync(Guid id);
        Task<PhieuNhapDetailedResponse> CreatePhieuNhapAsync(PhieuNhapCreateRequest request);
    }
}
