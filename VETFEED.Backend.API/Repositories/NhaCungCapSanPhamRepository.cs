using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.NhaCungCapSanPham;
using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public class NhaCungCapSanPhamRepository : INhaCungCapSanPhamRepository
    {
        private readonly VetFeedManagementContext _context;

        public NhaCungCapSanPhamRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        // Lấy tất cả liên kết NCC-SP
        public async Task<IEnumerable<NhaCungCapSanPhamResponse>> GetAllNhaCungCapSanPhamsAsync()
        {
            return await _context.NhaCungCapSanPhams
                .Include(x => x.NhaCungCap)
                .Include(x => x.SanPham)
                .Select(x => new NhaCungCapSanPhamResponse
                {
                    MaNCSP = x.MaNCSP,
                    MaNCC = x.MaNCC,
                    TenNCC = x.NhaCungCap != null ? x.NhaCungCap.TenNCC : null,
                    MaSP = x.MaSP,
                    TenSP = x.SanPham != null ? x.SanPham.TenSP : null,
                    GiaNhapMacDinh = x.GiaNhapMacDinh,
                    TrangThai = x.TrangThai.ToString(),
                    GhiChu = x.GhiChu,
                    NgayTao = x.NgayTao
                }).ToListAsync();
        }

        // Lấy theo ID
        public async Task<NhaCungCapSanPhamResponse?> GetNhaCungCapSanPhamByIdAsync(Guid id)
        {
            var x = await _context.NhaCungCapSanPhams
                .Include(n => n.NhaCungCap)
                .Include(n => n.SanPham)
                .FirstOrDefaultAsync(n => n.MaNCSP == id);

            if (x == null) return null;

            return new NhaCungCapSanPhamResponse
            {
                MaNCSP = x.MaNCSP,
                MaNCC = x.MaNCC,
                TenNCC = x.NhaCungCap?.TenNCC,
                MaSP = x.MaSP,
                TenSP = x.SanPham?.TenSP,
                GiaNhapMacDinh = x.GiaNhapMacDinh,
                TrangThai = x.TrangThai.ToString(),
                GhiChu = x.GhiChu,
                NgayTao = x.NgayTao
            };
        }

        // Lấy theo nhà cung cấp
        public async Task<IEnumerable<NhaCungCapSanPhamResponse>> GetByNhaCungCapAsync(Guid maNCC)
        {
            return await _context.NhaCungCapSanPhams
                .Include(x => x.NhaCungCap)
                .Include(x => x.SanPham)
                .Where(x => x.MaNCC == maNCC)
                .Select(x => new NhaCungCapSanPhamResponse
                {
                    MaNCSP = x.MaNCSP,
                    MaNCC = x.MaNCC,
                    TenNCC = x.NhaCungCap != null ? x.NhaCungCap.TenNCC : null,
                    MaSP = x.MaSP,
                    TenSP = x.SanPham != null ? x.SanPham.TenSP : null,
                    GiaNhapMacDinh = x.GiaNhapMacDinh,
                    TrangThai = x.TrangThai.ToString(),
                    GhiChu = x.GhiChu,
                    NgayTao = x.NgayTao
                }).ToListAsync();
        }

        // Lấy theo sản phẩm
        public async Task<IEnumerable<NhaCungCapSanPhamResponse>> GetBySanPhamAsync(Guid maSP)
        {
            return await _context.NhaCungCapSanPhams
                .Include(x => x.NhaCungCap)
                .Include(x => x.SanPham)
                .Where(x => x.MaSP == maSP)
                .Select(x => new NhaCungCapSanPhamResponse
                {
                    MaNCSP = x.MaNCSP,
                    MaNCC = x.MaNCC,
                    TenNCC = x.NhaCungCap != null ? x.NhaCungCap.TenNCC : null,
                    MaSP = x.MaSP,
                    TenSP = x.SanPham != null ? x.SanPham.TenSP : null,
                    GiaNhapMacDinh = x.GiaNhapMacDinh,
                    TrangThai = x.TrangThai.ToString(),
                    GhiChu = x.GhiChu,
                    NgayTao = x.NgayTao
                }).ToListAsync();
        }

        // Thêm mới
        public async Task<NhaCungCapSanPhamResponse> AddNhaCungCapSanPhamAsync(NhaCungCapSanPhamRequest request)
        {
            var entity = new NhaCungCapSanPham
            {
                MaNCSP = Guid.NewGuid(),
                MaNCC = request.MaNCC,
                MaSP = request.MaSP,
                GiaNhapMacDinh = request.GiaNhapMacDinh,
                TrangThai = Enum.Parse<Enums.TrangThaiNhaCungCapSanPhamEnum>(request.TrangThai!, true),
                GhiChu = request.GhiChu,
                NgayTao = DateTime.Now
            };

            _context.NhaCungCapSanPhams.Add(entity);
            await _context.SaveChangesAsync();

            // Load navigation properties
            await _context.Entry(entity).Reference(e => e.NhaCungCap).LoadAsync();
            await _context.Entry(entity).Reference(e => e.SanPham).LoadAsync();

            return new NhaCungCapSanPhamResponse
            {
                MaNCSP = entity.MaNCSP,
                MaNCC = entity.MaNCC,
                TenNCC = entity.NhaCungCap?.TenNCC,
                MaSP = entity.MaSP,
                TenSP = entity.SanPham?.TenSP,
                GiaNhapMacDinh = entity.GiaNhapMacDinh,
                TrangThai = entity.TrangThai.ToString(),
                GhiChu = entity.GhiChu,
                NgayTao = entity.NgayTao
            };
        }

        // Cập nhật
        public async Task<NhaCungCapSanPhamResponse?> UpdateNhaCungCapSanPhamAsync(Guid id, NhaCungCapSanPhamRequest request)
        {
            var entity = await _context.NhaCungCapSanPhams.FindAsync(id);
            if (entity == null) return null;

            entity.MaNCC = request.MaNCC;
            entity.MaSP = request.MaSP;
            entity.GiaNhapMacDinh = request.GiaNhapMacDinh;
            entity.TrangThai = Enum.Parse<Enums.TrangThaiNhaCungCapSanPhamEnum>(request.TrangThai!, true);
            entity.GhiChu = request.GhiChu;

            await _context.SaveChangesAsync();

            // Load navigation properties
            await _context.Entry(entity).Reference(e => e.NhaCungCap).LoadAsync();
            await _context.Entry(entity).Reference(e => e.SanPham).LoadAsync();

            return new NhaCungCapSanPhamResponse
            {
                MaNCSP = entity.MaNCSP,
                MaNCC = entity.MaNCC,
                TenNCC = entity.NhaCungCap?.TenNCC,
                MaSP = entity.MaSP,
                TenSP = entity.SanPham?.TenSP,
                GiaNhapMacDinh = entity.GiaNhapMacDinh,
                TrangThai = entity.TrangThai.ToString(),
                GhiChu = entity.GhiChu,
                NgayTao = entity.NgayTao
            };
        }

        // Xóa
        public async Task<bool> DeleteNhaCungCapSanPhamAsync(Guid id)
        {
            var entity = await _context.NhaCungCapSanPhams.FindAsync(id);
            if (entity == null) return false;

            _context.NhaCungCapSanPhams.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // Lấy entities theo nhà cung cấp (để xử lý trong transaction)
        public async Task<IEnumerable<NhaCungCapSanPham>> GetEntitiesByNhaCungCapAsync(Guid maNCC)
        {
            return await _context.NhaCungCapSanPhams
                .Where(x => x.MaNCC == maNCC)
                .ToListAsync();
        }

        // Xóa tất cả liên kết NCC-SP của một nhà cung cấp (cascade delete)
        public async Task<int> DeleteByNhaCungCapAsync(Guid maNCC)
        {
            var entities = await _context.NhaCungCapSanPhams
                .Where(x => x.MaNCC == maNCC)
                .ToListAsync();
            
            if (!entities.Any()) return 0;
            
            _context.NhaCungCapSanPhams.RemoveRange(entities);
            await _context.SaveChangesAsync();
            return entities.Count;
        }
    }
}

