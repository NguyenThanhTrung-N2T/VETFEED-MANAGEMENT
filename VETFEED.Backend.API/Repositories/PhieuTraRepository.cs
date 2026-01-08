using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.PhieuTra;

namespace VETFEED.Backend.API.Repositories
{
    public class PhieuTraRepository  : IPhieuTraRepository
    {
        private readonly VetFeedManagementContext _context;
        public PhieuTraRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        // lay danh sach phieu tra
        public async Task<IEnumerable<PhieuTraListResponse>> GetDanhSachPhieuTraAsync() 
        { 
            return await _context.PhieuTras
                .Include(pt => pt.KhachHang)
                .Include(pt => pt.PhieuBan)
                .Select(pt => new PhieuTraListResponse 
                { 
                    MaPT = pt.MaPT, 
                    MaPTCode = pt.MaPTCode!, 
                    NgayTra = pt.NgayTra, 
                    MaPB = pt.MaPB, 
                    MaPBCode = pt.PhieuBan!.MaPBCode, 
                    MaKH = pt.MaKH, 
                    TenKhachHang = pt.KhachHang!.TenKH, 
                    ThanhTien = pt.ThanhTien, 
                    HinhThucHoanTien = pt.HinhThucHoanTien.ToString() 
                })
                .OrderByDescending(pt => pt.NgayTra)
                .ToListAsync(); 
        }
    }
}
