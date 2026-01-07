using VETFEED.Backend.API.DTOs.NhaCungCapSanPham;

namespace VETFEED.Backend.API.Repositories
{
    public interface INhaCungCapSanPhamRepository
    {
        Task<IEnumerable<NhaCungCapSanPhamResponse>> GetAllNhaCungCapSanPhamsAsync();
        Task<NhaCungCapSanPhamResponse?> GetNhaCungCapSanPhamByIdAsync(Guid id);
        Task<IEnumerable<NhaCungCapSanPhamResponse>> GetByNhaCungCapAsync(Guid maNCC);
        Task<IEnumerable<NhaCungCapSanPhamResponse>> GetBySanPhamAsync(Guid maSP);
        Task<NhaCungCapSanPhamResponse> AddNhaCungCapSanPhamAsync(NhaCungCapSanPhamRequest request);
        Task<NhaCungCapSanPhamResponse?> UpdateNhaCungCapSanPhamAsync(Guid id, NhaCungCapSanPhamRequest request);
        Task<bool> DeleteNhaCungCapSanPhamAsync(Guid id);
    }
}
