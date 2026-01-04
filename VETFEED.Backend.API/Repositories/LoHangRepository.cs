using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.LoHang;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Utils;

namespace VETFEED.Backend.API.Repositories
{
    public class LoHangRepository : ILoHangRepository
    {
        private readonly VetFeedManagementContext _context;

        public LoHangRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        // Lấy tất cả lô hàng
        public async Task<IEnumerable<LoHangResponse>> GetAllLoHangsAsync()
        {
            return await _context.LoHangs
                .Include(x => x.SanPham)
                .Select(x => new LoHangResponse
                {
                    MaLo = x.MaLo,
                    MaLoCode = x.MaLoCode,
                    MaSP = x.MaSP,
                    TenSP = x.SanPham != null ? x.SanPham.TenSP : null,
                    NgaySanXuat = x.NgaySanXuat,
                    HanSuDung = x.HanSuDung
                }).ToListAsync();
        }

        // Lấy theo ID
        public async Task<LoHangResponse?> GetLoHangByIdAsync(Guid id)
        {
            var x = await _context.LoHangs
                .Include(l => l.SanPham)
                .FirstOrDefaultAsync(l => l.MaLo == id);

            if (x == null) return null;

            return new LoHangResponse
            {
                MaLo = x.MaLo,
                MaLoCode = x.MaLoCode,
                MaSP = x.MaSP,
                TenSP = x.SanPham?.TenSP,
                NgaySanXuat = x.NgaySanXuat,
                HanSuDung = x.HanSuDung
            };
        }

        // Lấy theo sản phẩm
        public async Task<IEnumerable<LoHangResponse>> GetBySanPhamAsync(Guid maSP)
        {
            return await _context.LoHangs
                .Include(x => x.SanPham)
                .Where(x => x.MaSP == maSP)
                .Select(x => new LoHangResponse
                {
                    MaLo = x.MaLo,
                    MaLoCode = x.MaLoCode,
                    MaSP = x.MaSP,
                    TenSP = x.SanPham != null ? x.SanPham.TenSP : null,
                    NgaySanXuat = x.NgaySanXuat,
                    HanSuDung = x.HanSuDung
                }).ToListAsync();
        }

        // Thêm mới
        public async Task<LoHangResponse> AddLoHangAsync(LoHangRequest request)
        {
            var entity = new LoHang
            {
                MaLo = Guid.NewGuid(),
                MaLoCode = await CodeGenerator.GenerateLoHangCodeAsync(_context),
                MaSP = request.MaSP,
                NgaySanXuat = request.NgaySanXuat,
                HanSuDung = request.HanSuDung
            };

            _context.LoHangs.Add(entity);
            await _context.SaveChangesAsync();

            // Load navigation property
            await _context.Entry(entity).Reference(e => e.SanPham).LoadAsync();

            return new LoHangResponse
            {
                MaLo = entity.MaLo,
                MaLoCode = entity.MaLoCode,
                MaSP = entity.MaSP,
                TenSP = entity.SanPham?.TenSP,
                NgaySanXuat = entity.NgaySanXuat,
                HanSuDung = entity.HanSuDung
            };
        }

        // Cập nhật
        public async Task<LoHangResponse?> UpdateLoHangAsync(Guid id, LoHangRequest request)
        {
            var entity = await _context.LoHangs.FindAsync(id);
            if (entity == null) return null;

            entity.MaSP = request.MaSP;
            entity.NgaySanXuat = request.NgaySanXuat;
            entity.HanSuDung = request.HanSuDung;

            await _context.SaveChangesAsync();

            // Load navigation property
            await _context.Entry(entity).Reference(e => e.SanPham).LoadAsync();

            return new LoHangResponse
            {
                MaLo = entity.MaLo,
                MaLoCode = entity.MaLoCode,
                MaSP = entity.MaSP,
                TenSP = entity.SanPham?.TenSP,
                NgaySanXuat = entity.NgaySanXuat,
                HanSuDung = entity.HanSuDung
            };
        }

        // Xóa
        public async Task<bool> DeleteLoHangAsync(Guid id)
        {
            var entity = await _context.LoHangs.FindAsync(id);
            if (entity == null) return false;

            _context.LoHangs.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
