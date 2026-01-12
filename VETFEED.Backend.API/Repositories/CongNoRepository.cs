using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.CongNo;
using VETFEED.Backend.API.Enums;

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
            // =========================
            // KHÁCH HÀNG
            // =========================
            var khQuery =
                from cn in _context.CongNos
                join kh in _context.KhachHangs
                    on cn.MaDoiTuong equals kh.MaKH
                where cn.LoaiDoiTuong == Enums.LoaiDoiTuongCongNoEnum.KHACH_HANG
                      && !(cn.SoTien < 0 && cn.GhiChu!.Contains("HOAN TIEN"))
                group cn by new
                {
                    kh.MaKH,
                    kh.MaKHCode,
                    kh.TenKH
                }
                into g
                select new
                {
                    MaDoiTuong = g.Key.MaKH,
                    MaDoiTuongCode = g.Key.MaKHCode,
                    TenDoiTuong = g.Key.TenKH,
                    LoaiDoiTuong = "KHACH_HANG",

                    TongPhatSinh = g
                        .Where(x => x.SoTien > 0)
                        .Sum(x => (decimal?)x.SoTien) ?? 0,

                    DaThanhToan = g
                        .Where(x => x.SoTien < 0)
                        .Sum(x => (decimal?)Math.Abs(x.SoTien)) ?? 0,

                    HanThanhToanGanNhat = g
                        .Where(x => x.SoTien > 0 && x.HanThanhToan != null)
                        .Min(x => x.HanThanhToan)
                };

            // =========================
            // NHÀ CUNG CẤP
            // =========================
            var nccQuery =
                from cn in _context.CongNos
                join ncc in _context.NhaCungCaps
                    on cn.MaDoiTuong equals ncc.MaNCC
                where cn.LoaiDoiTuong == Enums.LoaiDoiTuongCongNoEnum.NHA_CUNG_CAP
                group cn by new
                {
                    ncc.MaNCC,
                    ncc.MaNCCCode,
                    ncc.TenNCC
                }
                into g
                select new
                {
                    MaDoiTuong = g.Key.MaNCC,
                    MaDoiTuongCode = g.Key.MaNCCCode,
                    TenDoiTuong = g.Key.TenNCC,
                    LoaiDoiTuong = "NHA_CUNG_CAP",

                    TongPhatSinh = g
                        .Where(x => x.SoTien > 0)
                        .Sum(x => (decimal?)x.SoTien) ?? 0,

                    DaThanhToan = g
                        .Where(x => x.SoTien < 0)
                        .Sum(x => (decimal?)Math.Abs(x.SoTien)) ?? 0,

                    HanThanhToanGanNhat = g
                        .Where(x => x.SoTien > 0 && x.HanThanhToan != null)
                        .Min(x => x.HanThanhToan)
                };

            // =========================
            // GỘP + TÍNH DƯ NỢ
            // =========================
            var result = await khQuery
                .Union(nccQuery)
                .Select(x => new CongNoTongHopResponse
                {
                    MaDoiTuong = x.MaDoiTuong,
                    MaDoiTuongCode = x.MaDoiTuongCode,
                    TenDoiTuong = x.TenDoiTuong,
                    LoaiDoiTuong = x.LoaiDoiTuong,

                    TongPhatSinh = x.TongPhatSinh,
                    DaThanhToan = x.DaThanhToan,

                    DuNo = x.TongPhatSinh - x.DaThanhToan < 0
                        ? 0
                        : x.TongPhatSinh - x.DaThanhToan,

                    HanThanhToanGanNhat = x.HanThanhToanGanNhat,

                    CoQuaHan = x.HanThanhToanGanNhat != null
                        && x.HanThanhToanGanNhat < DateTime.Today
                })
                .ToListAsync();

            return result;
        }




    }
}
