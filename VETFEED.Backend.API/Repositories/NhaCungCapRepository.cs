using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.NhaCungCap;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Utils;

namespace VETFEED.Backend.API.Repositories
{
    public class NhaCungCapRepository : INhaCungCapRepository
    {
        private readonly VetFeedManagementContext _context;

        public NhaCungCapRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        // Lấy tất cả nhà cung cấp
        public async Task<IEnumerable<NhaCungCapResponse>> GetAllNhaCungCapsAsync()
        {
            return await _context.NhaCungCaps.Select(n => new NhaCungCapResponse
            {
                MaNCC = n.MaNCC,
                MaNCCCode = n.MaNCCCode,
                TenNCC = n.TenNCC,
                SoDienThoai = n.SoDienThoai,
                DiaChi = n.DiaChi,
                TrangThai = n.TrangThai.ToString(),
                GhiChu = n.GhiChu,
                NgayTao = n.NgayTao
            }).ToListAsync();
        }

        // Lấy nhà cung cấp theo ID
        public async Task<NhaCungCapResponse?> GetNhaCungCapByIdAsync(Guid id)
        {
            var n = await _context.NhaCungCaps.FindAsync(id);
            if (n == null) return null;

            return new NhaCungCapResponse
            {
                MaNCC = n.MaNCC,
                MaNCCCode = n.MaNCCCode,
                TenNCC = n.TenNCC,
                SoDienThoai = n.SoDienThoai,
                DiaChi = n.DiaChi,
                TrangThai = n.TrangThai.ToString(),
                GhiChu = n.GhiChu,
                NgayTao = n.NgayTao
            };
        }

        // Thêm nhà cung cấp mới
        public async Task<NhaCungCapResponse> AddNhaCungCapAsync(NhaCungCapRequest request)
        {
            var entity = new NhaCungCap
            {
                MaNCC = Guid.NewGuid(),
                MaNCCCode = await CodeGenerator.GenerateNhaCungCapCodeAsync(_context),
                TenNCC = request.TenNCC,
                SoDienThoai = request.SoDienThoai,
                DiaChi = request.DiaChi,
                TrangThai = request.TrangThai,
                GhiChu = request.GhiChu,
                NgayTao = DateTime.Now
            };

            _context.NhaCungCaps.Add(entity);
            await _context.SaveChangesAsync();

            return new NhaCungCapResponse
            {
                MaNCC = entity.MaNCC,
                MaNCCCode = entity.MaNCCCode,
                TenNCC = entity.TenNCC,
                SoDienThoai = entity.SoDienThoai,
                DiaChi = entity.DiaChi,
                TrangThai = entity.TrangThai.ToString(),
                GhiChu = entity.GhiChu,
                NgayTao = entity.NgayTao
            };
        }

        // Cập nhật nhà cung cấp
        public async Task<NhaCungCapResponse?> UpdateNhaCungCapAsync(Guid id, NhaCungCapRequest request)
        {
            var entity = await _context.NhaCungCaps.FindAsync(id);
            if (entity == null) return null;

            entity.TenNCC = request.TenNCC;
            entity.SoDienThoai = request.SoDienThoai;
            entity.DiaChi = request.DiaChi;
            entity.TrangThai = request.TrangThai;
            entity.GhiChu = request.GhiChu;

            await _context.SaveChangesAsync();

            return new NhaCungCapResponse
            {
                MaNCC = entity.MaNCC,
                MaNCCCode = entity.MaNCCCode,
                TenNCC = entity.TenNCC,
                SoDienThoai = entity.SoDienThoai,
                DiaChi = entity.DiaChi,
                TrangThai = entity.TrangThai.ToString(),
                GhiChu = entity.GhiChu,
                NgayTao = entity.NgayTao
            };
        }

        // Xóa nhà cung cấp
        public async Task<bool> DeleteNhaCungCapAsync(Guid id)
        {
            var entity = await _context.NhaCungCaps.FindAsync(id);
            if (entity == null) return false;

            _context.NhaCungCaps.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
