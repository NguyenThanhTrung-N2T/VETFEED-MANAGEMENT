using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.Common;
using VETFEED.Backend.API.DTOs.KhachHang;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.Models;


namespace VETFEED.Backend.API.Repositories
{
    public class KhachHangRepository : IKhachHangRepository
    {
        private readonly VetFeedManagementContext _context;
        public KhachHangRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<KhachHangResponse>> SearchAsync(KhachHangQuery query)
        {
            var q = _context.KhachHangs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var kw = query.Keyword.Trim();
                q = q.Where(x =>
                    (x.MaKHCode != null && x.MaKHCode.Contains(kw)) ||
                    (x.TenKH != null && x.TenKH.Contains(kw)) ||
                    (x.SoDienThoai != null && x.SoDienThoai.Contains(kw))
                );
            }

            if (!string.IsNullOrWhiteSpace(query.LoaiKhachHang))
            {
                if (Enum.TryParse<LoaiKhachHangEnum>(query.LoaiKhachHang.Trim(), true, out var loai))
                    q = q.Where(x => x.LoaiKhachHang == loai);
                else
                    return new PagedResult<KhachHangResponse>
                    {
                        Items = new List<KhachHangResponse>(),
                        Total = 0,
                        Page = query.Page,
                        PageSize = query.PageSize
                    };
            }

            if (!string.IsNullOrWhiteSpace(query.TrangThai))
            {
                if (Enum.TryParse<TrangThaiKhachHangEnum>(query.TrangThai.Trim(), true, out var tt))
                    q = q.Where(x => x.TrangThai == tt);
                else
                    return new PagedResult<KhachHangResponse>
                    {
                        Items = new List<KhachHangResponse>(),
                        Total = 0,
                        Page = query.Page,
                        PageSize = query.PageSize
                    };
            }


            var total = await q.CountAsync();

            q = q.OrderByDescending(x => x.NgayTao);

            if (query.Page.HasValue && query.PageSize.HasValue && query.Page > 0 && query.PageSize > 0)
            {
                q = q.Skip((query.Page.Value - 1) * query.PageSize.Value)
                     .Take(query.PageSize.Value);
            }

            var items = await q.Select(x => new KhachHangResponse
            {
                MaKH = x.MaKH,
                MaKHCode = x.MaKHCode,
                TenKH = x.TenKH,
                SoDienThoai = x.SoDienThoai,
                DiaChi = x.DiaChi,
                LoaiKhachHang = x.LoaiKhachHang.ToString(),
                HanMucCongNo = x.HanMucCongNo,
                TongMua = x.TongMua,
                CongNoHienTai = x.CongNoHienTai,
                TrangThai = x.TrangThai.ToString(),
                GhiChu = x.GhiChu,
                NgayTao = x.NgayTao
            }).ToListAsync();

            return new PagedResult<KhachHangResponse>
            {
                Items = items,
                Total = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<KhachHangResponse?> GetByIdAsync(Guid maKH)
        {
            return await _context.KhachHangs.AsNoTracking()
                .Where(x => x.MaKH == maKH)
                .Select(x => new KhachHangResponse
                {
                    MaKH = x.MaKH,
                    MaKHCode = x.MaKHCode,
                    TenKH = x.TenKH,
                    SoDienThoai = x.SoDienThoai,
                    DiaChi = x.DiaChi,
                    LoaiKhachHang = x.LoaiKhachHang.ToString(),
                    HanMucCongNo = x.HanMucCongNo,
                    TongMua = x.TongMua,
                    CongNoHienTai = x.CongNoHienTai,
                    TrangThai = x.TrangThai.ToString(),
                    GhiChu = x.GhiChu,
                    NgayTao = x.NgayTao
                }).FirstOrDefaultAsync();
        }

        public async Task<KhachHangResponse> CreateAsync(KhachHang entity)
        {
            _context.KhachHangs.Add(entity);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(entity.MaKH))!;
        }

        public async Task<KhachHangResponse?> UpdateAsync(Guid maKH, KhachHangUpdateRequest request)
        {
            var kh = await _context.KhachHangs.FirstOrDefaultAsync(x => x.MaKH == maKH);
            if (kh == null) return null;

            kh.TenKH = request.TenKH;
            kh.SoDienThoai = request.SoDienThoai;
            kh.DiaChi = request.DiaChi;
            kh.HanMucCongNo = request.HanMucCongNo;
            kh.GhiChu = request.GhiChu;

            // enum set ở service (parse)
            await _context.SaveChangesAsync();

            return await GetByIdAsync(maKH);
        }

        public async Task<bool> DeleteAsync(Guid maKH)
        {
            var kh = await _context.KhachHangs.FirstOrDefaultAsync(x => x.MaKH == maKH);
            if (kh == null) return false;

            _context.KhachHangs.Remove(kh);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasReferencesAsync(Guid maKH)
        {
            if (await _context.PhieuBans.AnyAsync(x => x.MaKH == maKH)) return true;
            if (await _context.PhieuTras.AnyAsync(x => x.MaKH == maKH)) return true;

            // CongNo là polymorphic (KH hoặc NCC)
            // Nếu model CongNo dùng enum LoaiDoiTuongCongNoEnum thì check như dưới:
            if (await _context.CongNos.AnyAsync(x =>
                x.LoaiDoiTuong == LoaiDoiTuongCongNoEnum.KHACH_HANG && x.MaDoiTuong == maKH))
                return true;

            return false;
        }

        public async Task<bool> PhoneExistsAsync(string phone, Guid? excludeMaKH = null)
        {
            var q = _context.KhachHangs.AsQueryable().Where(x => x.SoDienThoai == phone);
            if (excludeMaKH.HasValue)
                q = q.Where(x => x.MaKH != excludeMaKH.Value);

            return await q.AnyAsync();
        }
    }
}
