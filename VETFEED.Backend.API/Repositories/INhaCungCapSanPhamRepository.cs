using VETFEED.Backend.API.DTOs.NhaCungCapSanPham;
using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public interface INhaCungCapSanPhamRepository
    {
        Task<IEnumerable<NhaCungCapSanPhamResponse>> GetAllNhaCungCapSanPhamsAsync();
        Task<NhaCungCapSanPhamResponse?> GetNhaCungCapSanPhamByIdAsync(Guid id);
        Task<IEnumerable<NhaCungCapSanPhamResponse>> GetByNhaCungCapAsync(Guid maNCC);
        Task<IEnumerable<NhaCungCapSanPham>> GetEntitiesByNhaCungCapAsync(Guid maNCC);
        Task<IEnumerable<NhaCungCapSanPhamResponse>> GetBySanPhamAsync(Guid maSP);
        Task<NhaCungCapSanPhamResponse> AddNhaCungCapSanPhamAsync(NhaCungCapSanPhamRequest request);
        Task<NhaCungCapSanPhamResponse?> UpdateNhaCungCapSanPhamAsync(Guid id, NhaCungCapSanPhamRequest request);
        Task<bool> DeleteNhaCungCapSanPhamAsync(Guid id);
        /// <summary>
        /// Xóa tất cả liên kết NCC-SP của một nhà cung cấp (cascade delete)
        /// </summary>
        Task<int> DeleteByNhaCungCapAsync(Guid maNCC);
    }
}

