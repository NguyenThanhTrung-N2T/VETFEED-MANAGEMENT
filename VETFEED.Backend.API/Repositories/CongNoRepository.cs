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
            var khQuery =
                from cn in _context.CongNos
                join kh in _context.KhachHangs
                    on cn.MaDoiTuong equals kh.MaKH
                where cn.LoaiDoiTuong == LoaiDoiTuongCongNoEnum.KHACH_HANG
                group cn by new
                {
                    kh.MaKH,
                    kh.MaKHCode,
                    kh.TenKH
                }
                into g
                let tongPhatSinh = g.Where(x => x.SoTien > 0).Sum(x => (decimal?)x.SoTien) ?? 0
                let daThanhToan = g.Where(x => x.SoTien < 0).Sum(x => (decimal?)Math.Abs(x.SoTien)) ?? 0
                let duNo = tongPhatSinh - daThanhToan
                select new CongNoTongHopResponse
                {
                    MaDoiTuong = g.Key.MaKH,
                    MaDoiTuongCode = g.Key.MaKHCode,
                    TenDoiTuong = g.Key.TenKH,
                    LoaiDoiTuong = "KHACH_HANG",

                    TongPhatSinh = tongPhatSinh,
                    DaThanhToan = daThanhToan,
                    DuNo = duNo < 0 ? 0 : duNo,

                    // HẠN SỚM NHẤT CỦA CÁC KHOẢN NỢ CHƯA TRẢ HẾT
                    HanThanhToanGanNhat = duNo > 0
                        ? g.Where(x => x.SoTien > 0 && x.HanThanhToan != null)
                           .Min(x => x.HanThanhToan)
                        : null,

                    CoQuaHan = duNo > 0
                        && g.Where(x => x.SoTien > 0 && x.HanThanhToan != null)
                             .Min(x => x.HanThanhToan) < DateTime.Today
                };

            var nccQuery =
                from cn in _context.CongNos
                join ncc in _context.NhaCungCaps
                    on cn.MaDoiTuong equals ncc.MaNCC
                where cn.LoaiDoiTuong == LoaiDoiTuongCongNoEnum.NHA_CUNG_CAP
                group cn by new
                {
                    ncc.MaNCC,
                    ncc.MaNCCCode,
                    ncc.TenNCC
                }
                into g
                let tongPhatSinh = g.Where(x => x.SoTien > 0).Sum(x => (decimal?)x.SoTien) ?? 0
                let daThanhToan = g.Where(x => x.SoTien < 0).Sum(x => (decimal?)Math.Abs(x.SoTien)) ?? 0
                let duNo = tongPhatSinh - daThanhToan
                select new CongNoTongHopResponse
                {
                    MaDoiTuong = g.Key.MaNCC,
                    MaDoiTuongCode = g.Key.MaNCCCode,
                    TenDoiTuong = g.Key.TenNCC,
                    LoaiDoiTuong = "NHA_CUNG_CAP",

                    TongPhatSinh = tongPhatSinh,
                    DaThanhToan = daThanhToan,
                    DuNo = duNo < 0 ? 0 : duNo,

                    HanThanhToanGanNhat = duNo > 0
                        ? g.Where(x => x.SoTien > 0 && x.HanThanhToan != null)
                           .Min(x => x.HanThanhToan)
                        : null,

                    CoQuaHan = duNo > 0
                        && g.Where(x => x.SoTien > 0 && x.HanThanhToan != null)
                             .Min(x => x.HanThanhToan) < DateTime.Today
                };

            return await khQuery
                .Union(nccQuery)
                .ToListAsync();
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
