using VETFEED.Backend.API.DTOs.NhaCungCapSanPham;
using VETFEED.Backend.API.Enums;
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
            if (request.MaNCC == Guid.Empty)
                throw new ArgumentException("Mã nhà cung cấp không được để trống.");

            if (request.MaSP == Guid.Empty)
                throw new ArgumentException("Mã sản phẩm không được để trống.");

            if (!Enum.TryParse<TrangThaiNhaCungCapSanPhamEnum>(request.TrangThai, true, out _))
                throw new ArgumentException("TrangThai không hợp lệ. Chỉ nhận: HOAT_DONG | NGUNG_HOAT_DONG.");

            return await _repo.AddNhaCungCapSanPhamAsync(request);
        }

        public async Task<NhaCungCapSanPhamResponse?> UpdateNhaCungCapSanPhamAsync(Guid id, NhaCungCapSanPhamRequest request)
        {
            if (request.MaNCC == Guid.Empty)
                throw new ArgumentException("Mã nhà cung cấp không được để trống.");

            if (request.MaSP == Guid.Empty)
                throw new ArgumentException("Mã sản phẩm không được để trống.");

            if (!Enum.TryParse<TrangThaiNhaCungCapSanPhamEnum>(request.TrangThai, true, out _))
                throw new ArgumentException("TrangThai không hợp lệ. Chỉ nhận: HOAT_DONG | NGUNG_HOAT_DONG.");

            return await _repo.UpdateNhaCungCapSanPhamAsync(id, request);
        }

        public async Task<bool> DeleteNhaCungCapSanPhamAsync(Guid id)
        {
            return await _repo.DeleteNhaCungCapSanPhamAsync(id);
        }
    }
}
