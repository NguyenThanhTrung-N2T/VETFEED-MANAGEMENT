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

        //Lấy danh sách chi tiết đơn hàng doanh thu với phân trang
        public async Task<DoanhThuDonHangResponse> GetDoanhThuDonHangAsync(DateTime from, DateTime to, int page, int limit)
        {
            // Query chi tiết phiếu bán với các join cần thiết
            var query = _context.CTPhieuBans
                .Include(ct => ct.PhieuBan)
                    .ThenInclude(pb => pb!.KhachHang)
                .Include(ct => ct.LoHang)
                    .ThenInclude(lh => lh!.SanPham)
                .Where(ct => ct.PhieuBan!.NgayBan >= from && ct.PhieuBan.NgayBan <= to)
                .OrderByDescending(ct => ct.PhieuBan!.NgayBan);

            // Đếm tổng số items
            var totalItems = await query.CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling((double)totalItems / limit);

            // Lấy dữ liệu với phân trang
            var data = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(ct => new DoanhThuDonHangItemResponse
                {
                    MaPhieuBanCode = ct.PhieuBan!.MaPBCode,
                    Ngay = ct.PhieuBan.NgayBan,
                    TenSanPham = ct.LoHang!.SanPham!.TenSP,
                    MaSPCode = ct.LoHang.SanPham.MaSPCode,
                    TenKhachHang = ct.PhieuBan.KhachHang!.TenKH,
                    SoLuong = ct.SoLuong,
                    DonGia = ct.DonGia,
                    ThanhTien = ct.SoLuong * ct.DonGia
                })
                .ToListAsync();

            return new DoanhThuDonHangResponse
            {
                Data = data,
                Meta = new PaginationMeta
                {
                    Page = page,
                    Limit = limit,
                    Total_Items = totalItems,
                    Total_Pages = totalPages
                }
            };
        }

        // Lấy phân tích lợi nhuận theo khoảng thời gian
        public async Task<LoiNhuanPhanTichResponse> GetLoiNhuanPhanTichAsync(DateTime from, DateTime to)
        {
            // Query chi tiết phiếu bán với các join cần thiết
            var chiTietPhieuBans = await _context.CTPhieuBans
                .Include(ct => ct.PhieuBan)
                .Include(ct => ct.LoHang)
                    .ThenInclude(lh => lh!.SanPham)
                .Where(ct => ct.PhieuBan!.NgayBan >= from && ct.PhieuBan.NgayBan <= to)
                .ToListAsync();

            // Tính tổng quan
            var tongDoanhThu = chiTietPhieuBans.Sum(ct => ct.SoLuong * ct.DonGia);
            var tongChiPhi = chiTietPhieuBans.Sum(ct => ct.ThanhTienVon);
            var tongLoiNhuan = tongDoanhThu - tongChiPhi;
            var tiSuat = tongDoanhThu > 0 ? Math.Round((tongLoiNhuan / tongDoanhThu) * 100, 2) : 0;

            // Tính lợi nhuận theo sản phẩm và lấy top 5
            var topSanPham = chiTietPhieuBans
                .GroupBy(ct => new
                {
                    MaSP = ct.LoHang!.SanPham!.MaSP,
                    TenSP = ct.LoHang.SanPham.TenSP
                })
                .Select(g => new TopSanPhamLoiNhuanResponse
                {
                    TenSanPham = g.Key.TenSP,
                    DoanhThu = g.Sum(ct => ct.SoLuong * ct.DonGia),
                    ChiPhi = g.Sum(ct => ct.ThanhTienVon),
                    LoiNhuan = g.Sum(ct => ct.SoLuong * ct.DonGia) - g.Sum(ct => ct.ThanhTienVon)
                })
                .OrderByDescending(sp => sp.LoiNhuan)
                .Take(5)
                .ToList();

            return new LoiNhuanPhanTichResponse
            {
                TongQuan = new LoiNhuanTongQuanResponse
                {
                    DoanhThu = tongDoanhThu,
                    ChiPhi = tongChiPhi,
                    LoiNhuan = tongLoiNhuan,
                    TiSuat = tiSuat
                },
                TopSanPhamChart = topSanPham
            };
        }
    }
}
