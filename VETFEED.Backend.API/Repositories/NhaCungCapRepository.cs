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

        // Lấy tất cả nhà cung cấp (lọc bỏ các NCC đã soft delete)
        public async Task<IEnumerable<NhaCungCapResponse>> GetAllNhaCungCapsAsync()
        {
            return await _context.NhaCungCaps
                .Where(n => !n.IsDeleted)  // Lọc bỏ NCC đã xóa
                .Select(n => new NhaCungCapResponse
                {
                    MaNCC = n.MaNCC,
                    MaNCCCode = n.MaNCCCode,
                    TenNCC = n.TenNCC,
                    SoDienThoai = n.SoDienThoai,
                    DiaChi = n.DiaChi,
                    TrangThai = n.TrangThai.ToString(),
                    GhiChu = n.GhiChu,
                    NgayTao = n.NgayTao,
                    SanPhamCount = _context.NhaCungCapSanPhams.Count(sp => sp.MaNCC == n.MaNCC)
                }).ToListAsync();
        }

        // Lấy nhà cung cấp theo ID
        public async Task<NhaCungCapResponse?> GetNhaCungCapByIdAsync(Guid id)
        {
            var n = await _context.NhaCungCaps.FindAsync(id);
            if (n == null || n.IsDeleted) return null;  // Kiểm tra soft delete

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
                TrangThai = Enum.Parse<Enums.TrangThaiNhaCungCapEnum>(request.TrangThai!, true),
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
            entity.TrangThai = Enum.Parse<Enums.TrangThaiNhaCungCapEnum>(request.TrangThai!, true);
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

        // Xóa nhà cung cấp (Soft Delete)
        public async Task<bool> DeleteNhaCungCapAsync(Guid id)
        {
            var entity = await _context.NhaCungCaps.FindAsync(id);
            if (entity == null || entity.IsDeleted) return false;

            // Soft delete thay vì hard delete
            entity.IsDeleted = true;
            entity.NgayXoa = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
