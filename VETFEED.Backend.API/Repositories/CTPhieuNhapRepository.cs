using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.CTPhieuNhap;
using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public class CTPhieuNhapRepository : ICTPhieuNhapRepository
    {
        private readonly VetFeedManagementContext _context;

        public CTPhieuNhapRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        // Lấy tất cả chi tiết theo phiếu nhập
        public async Task<IEnumerable<CTPhieuNhapResponse>> GetAllByPhieuNhapAsync(Guid maPN)
        {
            return await _context.CTPhieuNhaps
                .Include(x => x.LoHang!)
                    .ThenInclude(l => l.SanPham)
                .Where(x => x.MaPN == maPN)
                .Select(x => new CTPhieuNhapResponse
                {
                    MaCTPN = x.MaCTPN,
                    MaPN = x.MaPN,
                    MaLo = x.MaLo,
                    MaLoCode = x.LoHang != null ? x.LoHang.MaLoCode : null,
                    TenSP = x.LoHang != null && x.LoHang.SanPham != null ? x.LoHang.SanPham.TenSP : null,
                    NgaySanXuat = x.LoHang != null ? x.LoHang.NgaySanXuat : null,
                    HanSuDung = x.LoHang != null ? x.LoHang.HanSuDung : null,
                    SoLuong = x.SoLuong,
                    DonGia = x.DonGia
                }).ToListAsync();
        }

        // Lấy chi tiết theo ID
        public async Task<CTPhieuNhapResponse?> GetCTPhieuNhapByIdAsync(Guid id)
        {
            var x = await _context.CTPhieuNhaps
                .Include(ct => ct.LoHang!)
                    .ThenInclude(l => l.SanPham)
                .FirstOrDefaultAsync(ct => ct.MaCTPN == id);

            if (x == null) return null;

            return new CTPhieuNhapResponse
            {
                MaCTPN = x.MaCTPN,
                MaPN = x.MaPN,
                MaLo = x.MaLo,
                MaLoCode = x.LoHang?.MaLoCode,
                TenSP = x.LoHang?.SanPham?.TenSP,
                NgaySanXuat = x.LoHang?.NgaySanXuat,
                HanSuDung = x.LoHang?.HanSuDung,
                SoLuong = x.SoLuong,
                DonGia = x.DonGia
            };
        }

        // Thêm chi tiết phiếu nhập - nhận MaLo đã có sẵn (Service tạo LoHang)
        public async Task<CTPhieuNhap> AddCTPhieuNhapAsync(Guid maPN, Guid maLo, decimal soLuong, decimal donGia = 0)
        {
            var entity = new CTPhieuNhap
            {
                MaCTPN = Guid.NewGuid(),
                MaPN = maPN,
                MaLo = maLo,
                SoLuong = soLuong,
                DonGia = donGia
            };

            _context.CTPhieuNhaps.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        // Xóa chi tiết phiếu nhập
        public async Task<bool> DeleteCTPhieuNhapAsync(Guid id)
        {
            var entity = await _context.CTPhieuNhaps.FindAsync(id);
            if (entity == null) return false;

            _context.CTPhieuNhaps.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        // Lay danh sach entities CTPhieuNhap (khong phai DTO) theo MaPN
        public async Task<IEnumerable<CTPhieuNhap>> GetCTPhieuNhapEntitiesByMaPNAsync(Guid maPN)
        {
            return await _context.CTPhieuNhaps
                .Include(ct => ct.LoHang)
                .Where(ct => ct.MaPN == maPN)
                .ToListAsync();
        }
    }
}
