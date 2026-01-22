using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.CongNo;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public class CongNoRepository : ICongNoRepository
    {
        private readonly VetFeedManagementContext _context;
        public CongNoRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        // lay cong nợ theo khach hang 
        public async Task<List<CongNoTongHopResponse>> GetTongHopCongNoAsync()
        {
            var today = DateTime.Today;

            // Lấy toàn bộ công nợ về memory
            var congNoList = await _context.CongNos
                .ToListAsync();

            var result = new List<CongNoTongHopResponse>();

            // KHÁCH HÀNG
            var khQuery = congNoList
                .Where(cn => cn.LoaiDoiTuong == LoaiDoiTuongCongNoEnum.KHACH_HANG)
                .Join(_context.KhachHangs, cn => cn.MaDoiTuong, kh => kh.MaKH, (cn, kh) => new { cn, kh })
                .GroupBy(x => new { x.kh.MaKH, x.kh.MaKHCode, x.kh.TenKH, x.kh.HanMucCongNo })
                .Select(g =>
                {
                    var tongPhatSinh = g.Where(x => x.cn.SoTien > 0).Sum(x => x.cn.SoTien);
                    var daThanhToan = g.Where(x => x.cn.SoTien < 0).Sum(x => Math.Abs(x.cn.SoTien));
                    var duNo = tongPhatSinh - daThanhToan;

                    DateTime? hanGanNhat = null;
                    if (duNo > 0)
                    {
                        hanGanNhat = g.Where(x => x.cn.SoTien > 0 && x.cn.HanThanhToan != null)
                                      .Min(x => x.cn.HanThanhToan);
                    }

                    return new CongNoTongHopResponse
                    {
                        MaDoiTuong = g.Key.MaKH,
                        MaDoiTuongCode = g.Key.MaKHCode!,
                        TenDoiTuong = g.Key.TenKH!,
                        LoaiDoiTuong = "KHACH_HANG",
                        TongPhatSinh = tongPhatSinh,
                        DaThanhToan = daThanhToan,
                        DuNo = duNo < 0 ? 0 : duNo,
                        HanMucCongNo = g.Key.HanMucCongNo ?? 0,
                        HanThanhToanGanNhat = hanGanNhat,
                        CoQuaHan = duNo > 0 && hanGanNhat != null && hanGanNhat < today
                    };
                });

            // NCC (lọc bỏ NCC đã soft delete)
            var nccQuery = congNoList
                .Where(cn => cn.LoaiDoiTuong == LoaiDoiTuongCongNoEnum.NHA_CUNG_CAP)
                .Join(_context.NhaCungCaps.Where(ncc => !ncc.IsDeleted), cn => cn.MaDoiTuong, ncc => ncc.MaNCC, (cn, ncc) => new { cn, ncc })
                .GroupBy(x => new { x.ncc.MaNCC, x.ncc.MaNCCCode, x.ncc.TenNCC })
                .Select(g =>
                {
                    var tongPhatSinh = g.Where(x => x.cn.SoTien > 0).Sum(x => x.cn.SoTien);
                    var daThanhToan = g.Where(x => x.cn.SoTien < 0).Sum(x => Math.Abs(x.cn.SoTien));
                    var duNo = tongPhatSinh - daThanhToan;

                    DateTime? hanGanNhat = null;
                    if (duNo > 0)
                    {
                        hanGanNhat = g.Where(x => x.cn.SoTien > 0 && x.cn.HanThanhToan != null)
                                      .Min(x => x.cn.HanThanhToan);
                    }

                    return new CongNoTongHopResponse
                    {
                        MaDoiTuong = g.Key.MaNCC,
                        MaDoiTuongCode = g.Key.MaNCCCode!,
                        TenDoiTuong = g.Key.TenNCC!,
                        LoaiDoiTuong = "NHA_CUNG_CAP",
                        TongPhatSinh = tongPhatSinh,
                        DaThanhToan = daThanhToan,
                        DuNo = duNo < 0 ? 0 : duNo,
                        HanMucCongNo = 0, // NCC không có hạn mức công nợ
                        HanThanhToanGanNhat = hanGanNhat,
                        CoQuaHan = duNo > 0 && hanGanNhat != null && hanGanNhat < today
                    };
                });

            result.AddRange(khQuery);
            result.AddRange(nccQuery);

            // Chỉ lấy đối tượng còn nợ
            return result.ToList();
        }



        // lay lịch sử công nợ của đối tượng theo mã đối tượng
        public async Task<List<CongNoHistoryRawDto>> GetCongNoHistoryAsync(Guid maDoiTuong)
        {
            var query =
                from cn in _context.CongNos
                where cn.MaDoiTuong == maDoiTuong
                   && (cn.GhiChu == null || !cn.GhiChu.ToUpper().Contains("HOAN TIEN"))

                join pb in _context.PhieuBans
                    on cn.MaPhieu equals pb.MaPB into pbj
                from pb in pbj.DefaultIfEmpty()

                join pt in _context.PhieuTras
                    on cn.MaPhieu equals pt.MaPT into ptj
                from pt in ptj.DefaultIfEmpty()

                join pn in _context.PhieuNhaps
                    on cn.MaPhieu equals pn.MaPN into pnj
                from pn in pnj.DefaultIfEmpty()

                orderby cn.NgayPhatSinh, cn.MaCongNo

                select new CongNoHistoryRawDto
                {
                    NgayPhatSinh = cn.NgayPhatSinh,
                    SoTien = cn.SoTien,
                    GhiChu = cn.GhiChu,

                    LoaiPhieu =
                        pb != null ? "PB" :
                        pt != null ? "PT" :
                        pn != null ? "PN" :
                        "OTHER",

                    MaPhieuCode =
                        pb != null ? pb.MaPBCode :
                        pt != null ? pt.MaPTCode :
                        pn != null ? pn.MaPNCode :
                        null
                };

            return await query.ToListAsync();
        }


        // lay khach hang ( de check han muc va cong no
        public async Task<KhachHang?> GetKhachHangByIdAsync(Guid maKH)
        {
            return await _context.KhachHangs
                .FirstOrDefaultAsync(x => x.MaKH == maKH);
        }

        // lay nha cung cap 
        public async Task<NhaCungCap?> GetNhaCungCapByIdAsync(Guid maNCC)
        {
            return await _context.NhaCungCaps
                .FirstOrDefaultAsync(x => x.MaNCC == maNCC);
        }

        // lay phieu ban theo ma code 
        public async Task<PhieuBan?> GetPhieuBanByCodeAsync(string maPBCode)
        {
            return await _context.PhieuBans
                .FirstOrDefaultAsync(x => x.MaPBCode == maPBCode);
        }

        // lay phieu nhap theo ma code
        public async Task<PhieuNhap?> GetPhieuNhapByCodeAsync(string maPNCode)
        {
            return await _context.PhieuNhaps
                .FirstOrDefaultAsync(x => x.MaPNCode == maPNCode);
        }

        // lay phieu ban theo ma ID
        public async Task<PhieuBan?> GetPhieuBanByIdAsync(Guid maPB)
        {
            return await _context.PhieuBans
                .FirstOrDefaultAsync(x => x.MaPB == maPB);
        }

        // lay phieu nhap theo ma ID
        public async Task<PhieuNhap?> GetPhieuNhapByIdAsync(Guid maPN)
        {
            return await _context.PhieuNhaps
                .FirstOrDefaultAsync(x => x.MaPN == maPN);
        }

        // tinh tong cong no theeo phieu 
        public async Task<decimal> GetTongCongNoTheoPhieuAsync(Guid maPhieu)
        {
            return await _context.CongNos
                .Where(x => x.MaPhieu == maPhieu)
                .SumAsync(x => x.SoTien);
        }


        // lay cong no chua tat toan theo ma doi tuong ( dung de giảm nợ khi khong có mã phiếu )
        public async Task<List<CongNo>> GetCongNoChuaTatToanAsync(Guid maDoiTuong)
        {
            return await _context.CongNos
                .Where(cn =>
                    cn.MaDoiTuong == maDoiTuong
                    && cn.SoTien > 0
                    && cn.HanThanhToan != null
                )
                .OrderBy(cn => cn.HanThanhToan)
                .ThenBy(cn => cn.NgayPhatSinh)
                .ToListAsync();
        }

        // them cong no moi
        public async Task AddCongNoAsync(CongNo congNo)
        {
            _context.CongNos.Add(congNo);
            await _context.SaveChangesAsync();
        }

        // luu thay doi
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


    }
}
