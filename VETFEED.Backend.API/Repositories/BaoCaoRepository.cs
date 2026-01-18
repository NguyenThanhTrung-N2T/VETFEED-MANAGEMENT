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

        // Lấy danh sách lợi nhuận theo sản phẩm với phân trang và sắp xếp
        public async Task<LoiNhuanSanPhamResponse> GetLoiNhuanSanPhamAsync(DateTime from, DateTime to, int page, int limit, string sortBy, string order)
        {
            // Query và group by sản phẩm
            var query = _context.CTPhieuBans
                .Include(ct => ct.PhieuBan)
                .Include(ct => ct.LoHang)
                    .ThenInclude(lh => lh!.SanPham)
                .Where(ct => ct.PhieuBan != null && ct.PhieuBan.NgayBan >= from && ct.PhieuBan.NgayBan <= to)
                .Where(ct => ct.LoHang != null && ct.LoHang.SanPham != null) // Null safety
                .GroupBy(ct => new
                {
                    MaSP = ct.LoHang!.SanPham!.MaSP,
                    MaSPCode = ct.LoHang.SanPham.MaSPCode,
                    TenSP = ct.LoHang.SanPham.TenSP
                })
                .Select(g => new
                {
                    MaSPCode = g.Key.MaSPCode,
                    TenSanPham = g.Key.TenSP,
                    SoLuongBan = g.Sum(ct => ct.SoLuongQuyDoi), // Dùng đơn vị cơ sở
                    DoanhThu = g.Sum(ct => ct.SoLuong * ct.DonGia),
                    ChiPhi = g.Sum(ct => ct.ThanhTienVon),
                    LoiNhuan = g.Sum(ct => ct.SoLuong * ct.DonGia) - g.Sum(ct => ct.ThanhTienVon),
                    TiSuat = g.Sum(ct => ct.SoLuong * ct.DonGia) > 0 
                        ? ((g.Sum(ct => ct.SoLuong * ct.DonGia) - g.Sum(ct => ct.ThanhTienVon)) / g.Sum(ct => ct.SoLuong * ct.DonGia)) * 100
                        : 0
                });

            // Sắp xếp theo doanh thu, số lượng, lợi nhuận, tỷ suất
            var sortedQuery = sortBy.ToLower() switch
            {
                "revenue" => order.ToLower() == "asc" 
                    ? query.OrderBy(p => p.DoanhThu)
                    : query.OrderByDescending(p => p.DoanhThu),
                "quantity" => order.ToLower() == "asc"
                    ? query.OrderBy(p => p.SoLuongBan)
                    : query.OrderByDescending(p => p.SoLuongBan),
                "margin" => order.ToLower() == "asc"
                    ? query.OrderBy(p => p.TiSuat)
                    : query.OrderByDescending(p => p.TiSuat),
                _ => order.ToLower() == "asc" // default: profit
                    ? query.OrderBy(p => p.LoiNhuan)
                    : query.OrderByDescending(p => p.LoiNhuan)
            };

            // Đếm tổng số sản phẩm
            var totalItems = await sortedQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / limit);

            // Áp dụng phân trang
            var data = await sortedQuery
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(p => new LoiNhuanSanPhamItemResponse
                {
                    MaSPCode = p.MaSPCode,
                    TenSanPham = p.TenSanPham,
                    SoLuongBan = p.SoLuongBan,
                    DoanhThu = p.DoanhThu,
                    ChiPhi = p.ChiPhi,
                    LoiNhuan = p.LoiNhuan,
                    TiSuat = Math.Round(p.TiSuat, 2)
                })
                .ToListAsync();

            return new LoiNhuanSanPhamResponse
            {
                Data = data ?? new List<LoiNhuanSanPhamItemResponse>(), 
                Meta = new PaginationMeta
                {
                    Page = page,
                    Limit = limit,
                    Total_Items = totalItems,
                    Total_Pages = totalPages
                }
            };
        }

        // Lấy phân tích tồn kho theo kho
        public async Task<TonKhoPhanTichResponse> GetTonKhoPhanTichAsync(Guid maKho)
        {
            // Query tồn kho theo kho với null safety
            var tonKhoData = await _context.TonKhos
                .Include(tk => tk.LoHang)
                    .ThenInclude(lh => lh!.SanPham)
                .Where(tk => tk.MaKho == maKho && tk.SoLuongCoSo > 0)
                .Where(tk => tk.LoHang != null && tk.LoHang.SanPham != null) // Null safety
                .ToListAsync();

            // Kiểm tra tồn kho
            if (!tonKhoData.Any())
            {
                return new TonKhoPhanTichResponse
                {
                    TongQuan = new TonKhoTongQuanResponse
                    {
                        TongSanPhamCount = 0,
                        TongSoLuong = 0,
                        SoLuongSapHetHan = 0
                    },
                    SoLuongChart = new List<SoLuongChartItemResponse>()
                };
            }

            // Tính tổng quan
            var tongSanPhamCount = tonKhoData
                .Select(tk => tk.LoHang!.SanPham!.MaSP)
                .Distinct()
                .Count();

            var tongSoLuong = tonKhoData.Sum(tk => tk.SoLuongCoSo);

            // Số lượng sắp hết hạn (< 30 ngày, chưa hết hạn)
            var today = DateTime.Now.Date;
            var expiringDate = today.AddDays(30);
            var soLuongSapHetHan = tonKhoData
                .Where(tk =>
                    tk.LoHang!.HanSuDung >= today &&
                    tk.LoHang.HanSuDung < expiringDate
                )
                .Select(tk => tk.LoHang!.SanPham!.MaSP)
                .Distinct()
                .Count();

            // Group by sản phẩm để tạo pie chart
            var productQuantities = tonKhoData
                .GroupBy(tk => new
                {
                    MaSP = tk.LoHang!.SanPham!.MaSP,
                    TenSP = tk.LoHang.SanPham.TenSP
                })
                .Select(g => new
                {
                    TenSanPham = g.Key.TenSP,
                    SoLuong = g.Sum(tk => tk.SoLuongCoSo)
                })
                .OrderByDescending(p => p.SoLuong)
                .ToList();

            // Logic pie chart: Top 7 + "Khác"
            List<SoLuongChartItemResponse> soLuongChart;

            if (productQuantities.Count > 7)
            {
                // Lấy top 7
                var top7 = productQuantities.Take(7)
                    .Select(p => new SoLuongChartItemResponse
                    {
                        TenSanPham = p.TenSanPham,
                        SoLuong = p.SoLuong
                    })
                    .ToList();

                // Tính tổng các sản phẩm còn lại
                var othersTotal = productQuantities.Skip(7).Sum(p => p.SoLuong);

                // Thêm slice "Khác"
                top7.Add(new SoLuongChartItemResponse
                {
                    TenSanPham = "Khác",
                    SoLuong = othersTotal
                });

                soLuongChart = top7;
            }
            else
            {
                // Nếu <= 7 sản phẩm, hiển thị tất cả
                soLuongChart = productQuantities
                    .Select(p => new SoLuongChartItemResponse
                    {
                        TenSanPham = p.TenSanPham,
                        SoLuong = p.SoLuong
                    })
                    .ToList();
            }

            return new TonKhoPhanTichResponse
            {
                TongQuan = new TonKhoTongQuanResponse
                {
                    TongSanPhamCount = tongSanPhamCount,
                    TongSoLuong = tongSoLuong,
                    SoLuongSapHetHan = soLuongSapHetHan
                },
                SoLuongChart = soLuongChart ?? new List<SoLuongChartItemResponse>() // Null safety
            };
        }

        // Lấy danh sách tồn kho sản phẩm với phân trang và lọc theo trạng thái
        public async Task<TonKhoSanPhamResponse> GetTonKhoSanPhamAsync(Guid maKho, int page, int limit, string trangThai)
        {
            var today = DateTime.Now.Date;
            var expiringDate = today.AddDays(30);

            // Query tồn kho với tính toán trạng thái
            var query = _context.TonKhos
                .Include(tk => tk.LoHang)
                    .ThenInclude(lh => lh!.SanPham)
                .Where(tk => tk.MaKho == maKho && tk.SoLuongCoSo > 0)
                .Where(tk => tk.LoHang != null && tk.LoHang.SanPham != null) // Null safety
                .Select(tk => new
                {
                    MaSP = tk.LoHang!.SanPham!.MaSP,
                    MaSPCode = tk.LoHang.SanPham.MaSPCode,
                    TenSanPham = tk.LoHang.SanPham.TenSP,
                    MaLoCode = tk.LoHang.MaLoCode,
                    SoLuong = tk.SoLuongCoSo,
                    DonVi = tk.LoHang.SanPham.DonViCoSo,
                    NgayHetHan = tk.LoHang.HanSuDung,
                    SoNgayDenKhiHetHan = (tk.LoHang.HanSuDung.Date - today).Days,
                    // Tính trạng thái
                    TrangThai = tk.LoHang.HanSuDung < today ? "HET_HAN"
                        : tk.LoHang.HanSuDung < expiringDate ? "SAP_HET_HAN"
                        : "CON_HAN"
                });

            // Apply status filter
            if (!string.IsNullOrEmpty(trangThai) && trangThai.ToUpper() != "ALL")
            {
                var statusFilter = trangThai.ToUpper() switch
                {
                    "CONHAN" => "CON_HAN",
                    "SAPHETHAN" => "SAP_HET_HAN",
                    "HETHAN" => "HET_HAN",
                    _ => trangThai.ToUpper()
                };
                query = query.Where(x => x.TrangThai == statusFilter);
            }

            // Sort by expiration date (soonest first)
            var sortedQuery = query.OrderBy(x => x.NgayHetHan);

            // Count total items before pagination
            var totalItems = await sortedQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / limit);

            // Apply pagination at database level
            var data = await sortedQuery
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(x => new TonKhoSanPhamItemResponse
                {
                    MaSP = x.MaSP,
                    MaSPCode = x.MaSPCode,
                    TenSanPham = x.TenSanPham,
                    MaLoCode = x.MaLoCode,
                    SoLuong = x.SoLuong,
                    DonVi = x.DonVi,
                    NgayHetHan = x.NgayHetHan,
                    TrangThai = x.TrangThai,
                    SoNgayDenKhiHetHan = x.SoNgayDenKhiHetHan
                })
                .ToListAsync();

            return new TonKhoSanPhamResponse
            {
                Data = data ?? new List<TonKhoSanPhamItemResponse>(), // Null safety
                Meta = new PaginationMeta
                {
                    Page = page,
                    Limit = limit,
                    Total_Items = totalItems,
                    Total_Pages = totalPages
                }
            };
        }
    }
}
