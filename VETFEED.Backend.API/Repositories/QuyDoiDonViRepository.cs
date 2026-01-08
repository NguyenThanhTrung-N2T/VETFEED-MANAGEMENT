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

        // Lấy danh sách quy đổi đơn vị theo mã sản phẩm
        public async Task<IEnumerable<QuyDoiDonVi>> GetByMaSPAsync(Guid maSP)
        {
            return await _context.QuyDoiDonVis
                .Where(q => q.MaSP == maSP)
                .ToListAsync();
        }

        // Lấy tỷ lệ quy đổi theo mã sản phẩm và đơn vị nhập
        public async Task<decimal?> GetTyLeByMaSPAndDonViNhapAsync(Guid maSP, string donViNhap)
        {
            var quyDoi = await _context.QuyDoiDonVis
                .FirstOrDefaultAsync(q => q.MaSP == maSP && q.DonViNhap == donViNhap);
            
            return quyDoi?.TyLe;
        }
    }
}
