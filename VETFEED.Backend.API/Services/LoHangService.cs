using VETFEED.Backend.API.DTOs.LoHang;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class LoHangService : ILoHangService
    {
        private readonly ILoHangRepository _repo;

        public LoHangService(ILoHangRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<LoHangResponse>> GetAllLoHangsAsync()
        {
            return await _repo.GetAllLoHangsAsync();
        }

        public async Task<LoHangResponse?> GetLoHangByIdAsync(Guid id)
        {
            return await _repo.GetLoHangByIdAsync(id);
        }

        public async Task<IEnumerable<LoHangResponse>> GetBySanPhamAsync(Guid maSP)
        {
            return await _repo.GetBySanPhamAsync(maSP);
        }

        public async Task<LoHangResponse> AddLoHangAsync(LoHangRequest request)
        {
            if (request.MaSP == Guid.Empty)
                throw new ArgumentException("Mã sản phẩm không được để trống.");

            if (request.NgaySanXuat.HasValue && request.HanSuDung <= request.NgaySanXuat.Value)
                throw new ArgumentException("Hạn sử dụng phải lớn hơn ngày sản xuất.");

            return await _repo.AddLoHangAsync(request);
        }

        public async Task<LoHangResponse?> UpdateLoHangAsync(Guid id, LoHangRequest request)
        {
            if (request.MaSP == Guid.Empty)
                throw new ArgumentException("Mã sản phẩm không được để trống.");

            if (request.NgaySanXuat.HasValue && request.HanSuDung <= request.NgaySanXuat.Value)
                throw new ArgumentException("Hạn sử dụng phải lớn hơn ngày sản xuất.");

            return await _repo.UpdateLoHangAsync(id, request);
        }

        public async Task<bool> DeleteLoHangAsync(Guid id)
        {
            return await _repo.DeleteLoHangAsync(id);
        }
    }
}
