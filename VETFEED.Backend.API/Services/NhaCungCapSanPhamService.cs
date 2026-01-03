using VETFEED.Backend.API.DTOs.NhaCungCapSanPham;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class NhaCungCapSanPhamService : INhaCungCapSanPhamService
    {
        private readonly INhaCungCapSanPhamRepository _repo;

        public NhaCungCapSanPhamService(INhaCungCapSanPhamRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<NhaCungCapSanPhamResponse>> GetAllNhaCungCapSanPhamsAsync()
        {
            return await _repo.GetAllNhaCungCapSanPhamsAsync();
        }

        public async Task<NhaCungCapSanPhamResponse?> GetNhaCungCapSanPhamByIdAsync(Guid id)
        {
            return await _repo.GetNhaCungCapSanPhamByIdAsync(id);
        }

        public async Task<IEnumerable<NhaCungCapSanPhamResponse>> GetByNhaCungCapAsync(Guid maNCC)
        {
            return await _repo.GetByNhaCungCapAsync(maNCC);
        }

        public async Task<IEnumerable<NhaCungCapSanPhamResponse>> GetBySanPhamAsync(Guid maSP)
        {
            return await _repo.GetBySanPhamAsync(maSP);
        }

        public async Task<NhaCungCapSanPhamResponse> AddNhaCungCapSanPhamAsync(NhaCungCapSanPhamRequest request)
        {
            return await _repo.AddNhaCungCapSanPhamAsync(request);
        }

        public async Task<NhaCungCapSanPhamResponse?> UpdateNhaCungCapSanPhamAsync(Guid id, NhaCungCapSanPhamRequest request)
        {
            return await _repo.UpdateNhaCungCapSanPhamAsync(id, request);
        }

        public async Task<bool> DeleteNhaCungCapSanPhamAsync(Guid id)
        {
            return await _repo.DeleteNhaCungCapSanPhamAsync(id);
        }
    }
}
