using VETFEED.Backend.API.DTOs.NhaCungCap;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class NhaCungCapService : INhaCungCapService
    {
        private readonly INhaCungCapRepository _repo;
        private readonly INhaCungCapSanPhamRepository _sanPhamRepo;

        public NhaCungCapService(INhaCungCapRepository repo, INhaCungCapSanPhamRepository sanPhamRepo)
        {
            _repo = repo;
            _sanPhamRepo = sanPhamRepo;
        }

        public async Task<IEnumerable<NhaCungCapResponse>> GetAllNhaCungCapsAsync()
        {
            return await _repo.GetAllNhaCungCapsAsync();
        }

        public async Task<NhaCungCapResponse?> GetNhaCungCapByIdAsync(Guid id)
        {
            var nhaCungCap = await _repo.GetNhaCungCapByIdAsync(id);
            if (nhaCungCap == null) return null;

            var sanPhams = await _sanPhamRepo.GetByNhaCungCapAsync(id);
            nhaCungCap.SanPhams = sanPhams.ToList();

            return nhaCungCap;
        }

        public async Task<NhaCungCapResponse> AddNhaCungCapAsync(NhaCungCapRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TenNCC))
                throw new ArgumentException("Tên nhà cung cấp không được để trống.");

            if (!Enum.IsDefined(typeof(TrangThaiNhaCungCapEnum), request.TrangThai))
                throw new ArgumentException("TrangThai không hợp lệ. Chỉ nhận: HOAT_DONG | NGUNG_HOAT_DONG.");

            return await _repo.AddNhaCungCapAsync(request);
        }

        public async Task<NhaCungCapResponse?> UpdateNhaCungCapAsync(Guid id, NhaCungCapRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TenNCC))
                throw new ArgumentException("Tên nhà cung cấp không được để trống.");

            if (!Enum.IsDefined(typeof(TrangThaiNhaCungCapEnum), request.TrangThai))
                throw new ArgumentException("TrangThai không hợp lệ. Chỉ nhận: HOAT_DONG | NGUNG_HOAT_DONG.");

            return await _repo.UpdateNhaCungCapAsync(id, request);
        }

        public async Task<bool> DeleteNhaCungCapAsync(Guid id)
        {
            return await _repo.DeleteNhaCungCapAsync(id);
        }
    }
}
