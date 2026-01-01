using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.KhoHang;

namespace VETFEED.Backend.API.Repositories
{
    public class KhoHangRepository : IKhoHangRepository
    {
        private readonly VetFeedManagementContext _context;
        public KhoHangRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        // lấy tất cả kho hàng
        public async Task<IEnumerable<KhoHangResponse>?> GetAllKhoHangsAsync()
        {
            // lấy tất cả kho hàng
            var khoHangs = await _context.KhoHangs.Select(k => new KhoHangResponse
            {
                MaKho = k.MaKho,
                MaKhoCode = k.MaKhoCode,
                TenKho = k.TenKho,
                DiaChi = k.DiaChi,
                TrangThai = k.TrangThai.ToString(),
                GhiChu = k.GhiChu
            }).ToListAsync();
            // trả về danh sách
            return khoHangs;
        }
    }
}
