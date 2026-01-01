using VETFEED.Backend.API.DTOs.KhoHang;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class KhoHangService : IKhoHangService
    {
        private readonly IKhoHangRepository _khoHangRepo;
        public KhoHangService(IKhoHangRepository khoHangsRepo)
        {
            _khoHangRepo = khoHangsRepo;
        }

        // lấy tất cả kho hàng 
        public async Task<IEnumerable<KhoHangResponse>?> GetAllKhoHangsAsync()
        {
            // lấy tất cả kho hàng từ db 
            var khoHangs = await _khoHangRepo.GetAllKhoHangsAsync();
            // trả về danh sách kho hàng 
            return khoHangs;
        }
    }
}
