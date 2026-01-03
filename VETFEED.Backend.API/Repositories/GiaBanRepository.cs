using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.Common;
using VETFEED.Backend.API.DTOs.GiaBan;
using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public class GiaBanRepository : IGiaBanRepository
    {
        private readonly VetFeedManagementContext _context;
        public GiaBanRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<GiaBanResponse>> SearchAsync(GiaBanQuery query)
        {
            var q = _context.GiaBans.AsNoTracking()
                .Include(x => x.SanPham)
                .AsQueryable();

            if (query.MaSP.HasValue)
                q = q.Where(x => x.MaSP == query.MaSP.Value);

            // filter theo khoảng ngày: lấy các dòng có giao với [from..to]
            if (query.From.HasValue || query.To.HasValue)
            {
                var from = query.From?.Date ?? DateTime.MinValue;
                var to = query.To.HasValue ? query.To.Value.Date.AddDays(1).AddTicks(-1) : DateTime.MaxValue;

                q = q.Where(x =>
                    x.TuNgay <= to &&
                    (x.DenNgay ?? DateTime.MaxValue) >= from
                );
            }

            var total = await q.CountAsync();
            q = q.OrderByDescending(x => x.TuNgay);

            if (query.Page.HasValue && query.PageSize.HasValue && query.Page > 0 && query.PageSize > 0)
            {
                q = q.Skip((query.Page.Value - 1) * query.PageSize.Value)
                     .Take(query.PageSize.Value);
            }

            var items = await q.Select(x => new GiaBanResponse
            {
                MaGia = x.MaGia,
                MaSP = x.MaSP,
                MaSPCode = x.SanPham != null ? x.SanPham.MaSPCode : null,
                TenSP = x.SanPham != null ? x.SanPham.TenSP : null,
                DonGiaBan = x.DonGiaBan,
                TuNgay = x.TuNgay,
                DenNgay = x.DenNgay,
                GhiChu = x.GhiChu,
                NgayTao = x.NgayTao
            }).ToListAsync();

            return new PagedResult<GiaBanResponse>
            {
                Items = items,
                Total = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<GiaBanResponse?> GetByIdAsync(Guid maGia)
        {
            return await _context.GiaBans.AsNoTracking()
                .Include(x => x.SanPham)
                .Where(x => x.MaGia == maGia)
                .Select(x => new GiaBanResponse
                {
                    MaGia = x.MaGia,
                    MaSP = x.MaSP,
                    MaSPCode = x.SanPham != null ? x.SanPham.MaSPCode : null,
                    TenSP = x.SanPham != null ? x.SanPham.TenSP : null,
                    DonGiaBan = x.DonGiaBan,
                    TuNgay = x.TuNgay,
                    DenNgay = x.DenNgay,
                    GhiChu = x.GhiChu,
                    NgayTao = x.NgayTao
                }).FirstOrDefaultAsync();
        }

        public async Task<GiaBanResponse> CreateAsync(GiaBan entity)
        {
            _context.GiaBans.Add(entity);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(entity.MaGia))!;
        }

        public async Task<GiaBanResponse?> UpdateAsync(Guid maGia, GiaBanUpdateRequest request)
        {
            var gb = await _context.GiaBans.FirstOrDefaultAsync(x => x.MaGia == maGia);
            if (gb == null) return null;

            gb.DonGiaBan = request.DonGiaBan;
            gb.TuNgay = request.TuNgay;
            gb.DenNgay = request.DenNgay;
            gb.GhiChu = request.GhiChu;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(maGia);
        }


        public async Task<bool> DeleteAsync(Guid maGia)
        {
            var gb = await _context.GiaBans.FirstOrDefaultAsync(x => x.MaGia == maGia);
            if (gb == null) return false;

            _context.GiaBans.Remove(gb);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<bool> ExistsSanPhamAsync(Guid maSP)
        {
            return _context.SanPhams.AnyAsync(x => x.MaSP == maSP);
        }

        public async Task<bool> IsOverlappingAsync(Guid maSP, DateTime tu, DateTime? den, Guid? excludeMaGia = null)
        {
            var newStart = tu;
            var newEnd = den ?? DateTime.MaxValue;

            var q = _context.GiaBans.AsNoTracking().Where(x => x.MaSP == maSP);
            if (excludeMaGia.HasValue)
                q = q.Where(x => x.MaGia != excludeMaGia.Value);

            // overlap inclusive: start1 <= end2 && start2 <= end1
            return await q.AnyAsync(x =>
                x.TuNgay <= newEnd &&
                (x.DenNgay ?? DateTime.MaxValue) >= newStart
            );
        }
        public async Task<GiaBanResponse?> GetCurrentPriceAsync(Guid maSP, DateTime date)
        {
            var d = date; // đã normalize ở service/controller nếu muốn

            return await _context.GiaBans.AsNoTracking()
                .Include(x => x.SanPham)
                .Where(x => x.MaSP == maSP
                    && x.TuNgay <= d
                    && (x.DenNgay == null || x.DenNgay >= d))
                .OrderByDescending(x => x.TuNgay)
                .Select(x => new GiaBanResponse
                {
                    MaGia = x.MaGia,
                    MaSP = x.MaSP,
                    MaSPCode = x.SanPham != null ? x.SanPham.MaSPCode : null,
                    TenSP = x.SanPham != null ? x.SanPham.TenSP : null,
                    DonGiaBan = x.DonGiaBan,
                    TuNgay = x.TuNgay,
                    DenNgay = x.DenNgay,
                    GhiChu = x.GhiChu,
                    NgayTao = x.NgayTao
                })
                .FirstOrDefaultAsync();
        }

    }
}
