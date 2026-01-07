using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.CTPhieuNhap;
using VETFEED.Backend.API.DTOs.PhieuNhap;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Utils;

namespace VETFEED.Backend.API.Repositories
{
    public class PhieuNhapRepository : IPhieuNhapRepository
    {
        private readonly VetFeedManagementContext _context;

        public PhieuNhapRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        // Lấy tất cả phiếu nhập
        public async Task<IEnumerable<PhieuNhapResponse>> GetAllPhieuNhapsAsync()
        {
            return await _context.PhieuNhaps
                .Include(x => x.NhaCungCap)
                .Include(x => x.KhoHang)
                .Select(x => new PhieuNhapResponse
                {
                    MaPN = x.MaPN,
                    MaPNCode = x.MaPNCode,
                    MaNCC = x.MaNCC,
                    TenNCC = x.NhaCungCap != null ? x.NhaCungCap.TenNCC : null,
                    MaKho = x.MaKho,
                    TenKho = x.KhoHang != null ? x.KhoHang.TenKho : null,
                    ThanhTien = x.ThanhTien,
                    TrangThai = x.TrangThai.ToString(),
                    GhiChu = x.GhiChu,
                    NgayCapNhat = x.NgayCapNhat
                }).ToListAsync();
        }

        // Lấy phiếu nhập theo ID (bao gồm chi tiết)
        public async Task<PhieuNhapDetailedResponse?> GetPhieuNhapByIdAsync(Guid id)
        {
            var x = await _context.PhieuNhaps
                .Include(p => p.NhaCungCap)
                .Include(p => p.KhoHang)
                .Include(p => p.CTPhieuNhaps!)
                    .ThenInclude(ct => ct.LoHang!)
                    .ThenInclude(l => l.SanPham)
                .FirstOrDefaultAsync(p => p.MaPN == id);

            if (x == null) return null;

            return new PhieuNhapDetailedResponse
            {
                MaPN = x.MaPN,
                MaPNCode = x.MaPNCode,
                MaNCC = x.MaNCC,
                TenNCC = x.NhaCungCap?.TenNCC,
                MaKho = x.MaKho,
                TenKho = x.KhoHang?.TenKho,
                ThanhTien = x.ThanhTien,
                TrangThai = x.TrangThai.ToString(),
                GhiChu = x.GhiChu,
                NgayCapNhat = x.NgayCapNhat,
                DanhSachChiTiet = x.CTPhieuNhaps?.Select(ct => new CTPhieuNhapResponse
                {
                    MaCTPN = ct.MaCTPN,
                    MaPN = ct.MaPN,
                    MaLo = ct.MaLo,
                    MaLoCode = ct.LoHang?.MaLoCode,
                    TenSP = ct.LoHang?.SanPham?.TenSP,
                    NgaySanXuat = ct.LoHang?.NgaySanXuat,
                    HanSuDung = ct.LoHang?.HanSuDung,
                    SoLuong = ct.SoLuong,
                    DonGia = ct.DonGia
                }).ToList()
            };
        }

        // Thêm phiếu nhập mới
        public async Task<PhieuNhapResponse> AddPhieuNhapAsync(PhieuNhapRequest request)
        {
            var entity = new PhieuNhap
            {
                MaPN = Guid.NewGuid(),
                MaPNCode = await CodeGenerator.GeneratePhieuNhapCodeAsync(_context),
                MaNCC = request.MaNCC,
                MaKho = request.MaKho,
                ThanhTien = 0, // Sẽ được cập nhật khi thêm chi tiết
                TrangThai = Enum.Parse<TrangThaiPhieuNhapEnum>(request.TrangThai ?? "DA_DAT", true),
                GhiChu = request.GhiChu,
                NgayCapNhat = DateTime.Now
            };

            _context.PhieuNhaps.Add(entity);
            await _context.SaveChangesAsync();

            // Load navigation properties
            await _context.Entry(entity).Reference(e => e.NhaCungCap).LoadAsync();
            await _context.Entry(entity).Reference(e => e.KhoHang).LoadAsync();

            return new PhieuNhapResponse
            {
                MaPN = entity.MaPN,
                MaPNCode = entity.MaPNCode,
                MaNCC = entity.MaNCC,
                TenNCC = entity.NhaCungCap?.TenNCC,
                MaKho = entity.MaKho,
                TenKho = entity.KhoHang?.TenKho,
                ThanhTien = entity.ThanhTien,
                TrangThai = entity.TrangThai.ToString(),
                GhiChu = entity.GhiChu,
                NgayCapNhat = entity.NgayCapNhat
            };
        }

        // Cập nhật phiếu nhập
        public async Task<PhieuNhapResponse?> UpdatePhieuNhapAsync(Guid id, PhieuNhapRequest request)
        {
            var entity = await _context.PhieuNhaps.FindAsync(id);
            if (entity == null) return null;

            entity.MaNCC = request.MaNCC;
            entity.MaKho = request.MaKho;
            entity.TrangThai = Enum.Parse<TrangThaiPhieuNhapEnum>(request.TrangThai ?? "DA_DAT", true);
            entity.GhiChu = request.GhiChu;
            entity.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();

            // Load navigation properties
            await _context.Entry(entity).Reference(e => e.NhaCungCap).LoadAsync();
            await _context.Entry(entity).Reference(e => e.KhoHang).LoadAsync();

            return new PhieuNhapResponse
            {
                MaPN = entity.MaPN,
                MaPNCode = entity.MaPNCode,
                MaNCC = entity.MaNCC,
                TenNCC = entity.NhaCungCap?.TenNCC,
                MaKho = entity.MaKho,
                TenKho = entity.KhoHang?.TenKho,
                ThanhTien = entity.ThanhTien,
                TrangThai = entity.TrangThai.ToString(),
                GhiChu = entity.GhiChu,
                NgayCapNhat = entity.NgayCapNhat
            };
        }

        // Xóa phiếu nhập
        public async Task<bool> DeletePhieuNhapAsync(Guid id)
        {
            var entity = await _context.PhieuNhaps
                .Include(p => p.CTPhieuNhaps)
                .FirstOrDefaultAsync(p => p.MaPN == id);
            
            if (entity == null) return false;

            // Xóa các chi tiết phiếu nhập trước
            if (entity.CTPhieuNhaps != null && entity.CTPhieuNhaps.Any())
            {
                _context.CTPhieuNhaps.RemoveRange(entity.CTPhieuNhaps);
            }

            _context.PhieuNhaps.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // Cập nhật tổng tiền phiếu nhập
        public async Task UpdateThanhTienAsync(Guid maPN)
        {
            var phieuNhap = await _context.PhieuNhaps
                .Include(p => p.CTPhieuNhaps)
                .FirstOrDefaultAsync(p => p.MaPN == maPN);

            if (phieuNhap != null && phieuNhap.CTPhieuNhaps != null)
            {
                phieuNhap.ThanhTien = phieuNhap.CTPhieuNhaps.Sum(ct => ct.SoLuong * (ct.DonGia ?? 0));
                phieuNhap.NgayCapNhat = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PhieuNhap?> GetPhieuNhapEntityByIdAsync(Guid id)
        {
            return await _context.PhieuNhaps
                .Include(p => p.CTPhieuNhaps!)
                    .ThenInclude(ct => ct.LoHang)
                .FirstOrDefaultAsync(p => p.MaPN == id);
        }

        // Cap nhat ThanhTien va TrangThai cho PhieuNhap
        public async Task<bool> UpdatePhieuNhapThanhTienAndTrangThaiAsync(Guid id, decimal thanhTien, TrangThaiPhieuNhapEnum trangThai)
        {
            var entity = await _context.PhieuNhaps.FindAsync(id);
            if (entity == null) return false;

            entity.ThanhTien = thanhTien;
            entity.TrangThai = trangThai;
            entity.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
