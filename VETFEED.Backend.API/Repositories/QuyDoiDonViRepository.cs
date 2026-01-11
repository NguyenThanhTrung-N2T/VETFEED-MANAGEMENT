using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public class QuyDoiDonViRepository : IQuyDoiDonViRepository
    {
        private readonly VetFeedManagementContext _context;

        public QuyDoiDonViRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QuyDoiDonVi>> GetByMaSPAsync(Guid maSP)
        {
            return await _context.QuyDoiDonVis
                .AsNoTracking()
                .Where(q => q.MaSP == maSP)
                .OrderBy(q => q.DonViNhap)
                .ToListAsync();
        }

        public async Task<decimal?> GetTyLeByMaSPAndDonViNhapAsync(Guid maSP, string donViNhap)
        {
            var quyDoi = await _context.QuyDoiDonVis
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.MaSP == maSP && q.DonViNhap == donViNhap);

            return quyDoi?.TyLe;
        }

        public async Task<QuyDoiDonVi?> GetByIdAsync(Guid maQD)
        {
            return await _context.QuyDoiDonVis
                .FirstOrDefaultAsync(q => q.MaQD == maQD);
        }

        public async Task<QuyDoiDonVi> AddAsync(QuyDoiDonVi entity)
        {
            _context.QuyDoiDonVis.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<QuyDoiDonVi?> UpdateAsync(QuyDoiDonVi entity)
        {
            var existing = await _context.QuyDoiDonVis.FirstOrDefaultAsync(q => q.MaQD == entity.MaQD);
            if (existing == null) return null;

            existing.DonViNhap = entity.DonViNhap;
            existing.TyLe = entity.TyLe;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid maQD)
        {
            var existing = await _context.QuyDoiDonVis.FirstOrDefaultAsync(q => q.MaQD == maQD);
            if (existing == null) return false;

            _context.QuyDoiDonVis.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsDonViNhapAsync(Guid maSP, string donViNhap, Guid? excludeMaQD = null)
        {
            var query = _context.QuyDoiDonVis
                .AsNoTracking()
                .Where(q => q.MaSP == maSP && q.DonViNhap == donViNhap);

            if (excludeMaQD.HasValue)
                query = query.Where(q => q.MaQD != excludeMaQD.Value);

            return await query.AnyAsync();
        }
    }
}
