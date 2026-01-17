using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.BaoCao;

namespace VETFEED.Backend.API.Repositories
{
    public class BaoCaoRepository : IBaoCaoRepository
    {
        private readonly VetFeedManagementContext _context;

        public BaoCaoRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        // doanh thu từ ngày from đến ngày to
        public async Task<DoanhThuPhanTichResponse> GetDoanhThuPhanTichAsync(DateTime from, DateTime to)
        {
            // Lấy danh sách phiếu bán trong khoảng thời gian
            var phieuBans = await _context.PhieuBans
                .Where(pb => pb.NgayBan >= from && pb.NgayBan <= to)
                .Include(pb => pb.CTPhieuBans)
                .ToListAsync();

            // Tính tổng quan
            var tongDoanhThu = phieuBans.Sum(pb => pb.ThanhTien);
            var tongDonHangCount = phieuBans.Count;
            var tongSanPhamCount = phieuBans
                .SelectMany(pb => pb.CTPhieuBans!)
                .Sum(ct => ct.SoLuongQuyDoi);

            // Tạo dữ liệu biểu đồ xu hướng theo ngày
            var xuHuongChart = phieuBans
                .GroupBy(pb => pb.NgayBan.Date)
                .Select(g => new XuHuongChartItemResponse
                {
                    Ngay = g.Key.ToString("yyyy-MM-dd"),
                    DoanhThu = g.Sum(pb => pb.ThanhTien),
                    DonHangCount = g.Count()
                })
                .OrderBy(x => x.Ngay)
                .ToList();

            return new DoanhThuPhanTichResponse
            {
                TongQuan = new TongQuanResponse
                {
                    TongDoanhThu = tongDoanhThu,
                    TongDonHangCount = tongDonHangCount,
                    TongSanPhamCount = tongSanPhamCount
                },
                XuHuongChart = xuHuongChart
            };
        }
    }
}
