using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.Repositories
{
    /// <summary>
    /// Repository for Dashboard data access operations
    /// </summary>
    public class DashboardRepository : IDashboardRepository
    {
        private readonly VetFeedManagementContext _context;

        public DashboardRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<decimal> GetRevenueByDateAsync(DateTime date)
        {
            return await _context.PhieuBans
                .Where(pb => pb.NgayBan.Date == date.Date && pb.TrangThaiThanhToan == TrangThaiThanhToanEnum.DA_THANH_TOAN)
                .SumAsync(pb => pb.ThanhTien);
        }

        /// <inheritdoc />
        public async Task<int> GetOrderCountByDateAsync(DateTime date)
        {
            return await _context.PhieuBans
                .CountAsync(pb => pb.NgayBan.Date == date.Date);
        }

        /// <inheritdoc />
        public async Task<decimal> GetTotalInventoryAsync()
        {
            return await _context.TonKhos
                .SumAsync(tk => tk.SoLuongCoSo);
        }

        /// <inheritdoc />
        public async Task<Dictionary<int, decimal>> GetMonthlyRevenueAsync(int year)
        {
            var monthlyData = await _context.PhieuBans
                .Where(pb => pb.NgayBan.Year == year && pb.TrangThaiThanhToan == TrangThaiThanhToanEnum.DA_THANH_TOAN)
                .GroupBy(pb => pb.NgayBan.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Revenue = g.Sum(pb => pb.ThanhTien)
                })
                .ToListAsync();

            return monthlyData.ToDictionary(x => x.Month, x => x.Revenue);
        }

        /// <inheritdoc />
        public async Task<List<ExpiringBatchInfo>> GetExpiringBatchesAsync(DateTime thresholdDate, int limit)
        {
            var expiringBatches = await (
                from lh in _context.LoHangs
                join sp in _context.SanPhams on lh.MaSP equals sp.MaSP
                join tk in _context.TonKhos on lh.MaLo equals tk.MaLo into tonKhoGroup
                where lh.HanSuDung <= thresholdDate && lh.HanSuDung >= DateTime.Today
                let tongSoLuong = tonKhoGroup.Sum(t => t.SoLuongCoSo)
                where tongSoLuong > 0
                orderby lh.HanSuDung ascending
                select new ExpiringBatchInfo
                {
                    MaLo = lh.MaLo,
                    MaLoCode = lh.MaLoCode,
                    TenSanPham = sp.TenSP,
                    LoaiSanPham = (int)sp.LoaiSanPham,
                    HanSuDung = lh.HanSuDung,
                    SoLuongTon = tongSoLuong
                }
            ).Take(limit).ToListAsync();

            return expiringBatches;
        }
    }
}
