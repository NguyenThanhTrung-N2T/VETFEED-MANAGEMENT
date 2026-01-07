using VETFEED.Backend.API.DTOs.NhaCungCap;

namespace VETFEED.Backend.API.Repositories
{
    public interface INhaCungCapRepository
    {
        Task<IEnumerable<NhaCungCapResponse>> GetAllNhaCungCapsAsync();
        Task<NhaCungCapResponse?> GetNhaCungCapByIdAsync(Guid id);
        Task<NhaCungCapResponse> AddNhaCungCapAsync(NhaCungCapRequest nhaCungCap);
        Task<NhaCungCapResponse?> UpdateNhaCungCapAsync(Guid id, NhaCungCapRequest nhaCungCap);
        Task<bool> DeleteNhaCungCapAsync(Guid id);
    }
}