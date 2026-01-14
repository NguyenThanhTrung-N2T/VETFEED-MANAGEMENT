using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.Common;
using VETFEED.Backend.API.DTOs.SanPham;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Enums;
using System;
using VETFEED.Backend.API.DTOs.QuyDoiDonVi; 
namespace VETFEED.Backend.API.Repositories
{
    public class SanPhamRepository : ISanPhamRepository
    {
        private readonly VetFeedManagementContext _context;
        public SanPhamRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<SanPhamResponse>> SearchAsync(SanPhamQuery query)
        {
            var q = _context.SanPhams
            .AsNoTracking()
            .Where(x => x.TrangThai == TrangThaiSanPhamEnum.HoatDong)
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var kw = query.Keyword.Trim();
                q = q.Where(x => (x.MaSPCode != null && x.MaSPCode.Contains(kw)) ||
                                 (x.TenSP != null && x.TenSP.Contains(kw)));
            }

           if (!string.IsNullOrWhiteSpace(query.LoaiSanPham))
            {
                if (Enum.TryParse<LoaiSanPhamEnum>(query.LoaiSanPham.Trim(), true, out var loai))
                {
                    q = q.Where(x => x.LoaiSanPham == loai);
                }
                else
                {
                    // truyền sai enum => trả rỗng (hoặc bạn có thể throw 400 ở service/controller)
                    return new PagedResult<SanPhamResponse>
                    {
                        Items = new List<SanPhamResponse>(),
                        Total = 0,
                        Page = query.Page,
                        PageSize = query.PageSize
                    };
                }
            }


            var total = await q.CountAsync();

            if (query.Page.HasValue && query.PageSize.HasValue
                && query.Page.Value > 0 && query.PageSize.Value > 0)
            {
                q = q.OrderByDescending(x => x.NgayTao)
                    .Skip((query.Page.Value - 1) * query.PageSize.Value)
                    .Take(query.PageSize.Value);
            }
            else
            {
                q = q.OrderByDescending(x => x.NgayTao);
            }

            var items = await q
            .Include(x => x.QuyDoiDonVis)
            .Select(x => new SanPhamResponse
            {
                MaSP = x.MaSP,
                MaSPCode = x.MaSPCode,
                TenSP = x.TenSP,
                LoaiSanPham = x.LoaiSanPham.ToString(),
                DonViCoSo = x.DonViCoSo,
                GhiChu = x.GhiChu,
                NgayTao = x.NgayTao,
                TrangThai = x.TrangThai.ToString(),
                DonGia = _context.GiaBans
                    .Where(g => g.MaSP == x.MaSP && g.DenNgay == null)
                    .OrderByDescending(g => g.TuNgay)
                    .Select(g => (decimal?)g.DonGiaBan)
                    .FirstOrDefault(),
                DonViQuyDoi = x.QuyDoiDonVis
                    .Select(q => new DonViQuyDoiItem
                    {
                        DonViNhap = q.DonViNhap ?? string.Empty,
                        TyLe = q.TyLe
                    }).ToList()
            })
            .ToListAsync();


            return new PagedResult<SanPhamResponse>
            {
                Items = items,
                Total = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<SanPhamResponse?> GetByIdAsync(Guid maSP)
        {
           return await _context.SanPhams.AsNoTracking()
            .Include(x => x.QuyDoiDonVis)
            .Where(x => x.MaSP == maSP)
            .Select(x => new SanPhamResponse
            {
                MaSP = x.MaSP,
                MaSPCode = x.MaSPCode,
                TenSP = x.TenSP,
                LoaiSanPham = x.LoaiSanPham.ToString(),
                DonViCoSo = x.DonViCoSo,
                GhiChu = x.GhiChu,
                NgayTao = x.NgayTao,
                TrangThai = x.TrangThai.ToString(),
                DonGia = _context.GiaBans
                    .Where(g => g.MaSP == x.MaSP && g.DenNgay == null)
                    .OrderByDescending(g => g.TuNgay)
                    .Select(g => (decimal?)g.DonGiaBan)
                    .FirstOrDefault(),
                DonViQuyDoi = x.QuyDoiDonVis
                    .Select(q => new DonViQuyDoiItem
                    {
                        DonViNhap = q.DonViNhap ?? string.Empty,
                        TyLe = q.TyLe
                    }).ToList()
            })
            .FirstOrDefaultAsync();

        }

        public async Task<SanPhamResponse> CreateAsync(SanPham entity)
        {
            _context.SanPhams.Add(entity);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(entity.MaSP))!;
        }

        public async Task<SanPhamResponse?> UpdateAsync(Guid maSP, SanPhamUpdateRequest request)
        {
            var sp = await _context.SanPhams.FirstOrDefaultAsync(x => x.MaSP == maSP);
            if (sp == null) return null;

            sp.TenSP = request.TenSP;
            sp.DonViCoSo = request.DonViTinh;
            sp.GhiChu = request.GhiChu;
            // LoaiSanPham sẽ set ở service (parse enum)
            await _context.SaveChangesAsync();

            return await GetByIdAsync(maSP);
        }

        public async Task<bool> DeleteAsync(Guid maSP)
        {
            var sp = await _context.SanPhams.FirstOrDefaultAsync(x => x.MaSP == maSP);
            if (sp == null) return false;

            sp.TrangThai = TrangThaiSanPhamEnum.KhongHoatDong;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasReferencesAsync(Guid maSP)
        {
            // 2. Có lô nào của sản phẩm không?
            var hasLo = await _context.LoHangs.AnyAsync(x => x.MaSP == maSP);
            if (hasLo) return true;

            // 3. Có map NCC–SP không?
            var hasNcsp = await _context.NhaCungCapSanPhams.AnyAsync(x => x.MaSP == maSP);
            return hasNcsp;
        }


        public async Task<SanPhamResponse?> GetByCodeAsync(string maSPCode)
        {
            if (string.IsNullOrWhiteSpace(maSPCode))
                return null;

            var code = maSPCode.Trim();

            return await _context.SanPhams.AsNoTracking()
                .Include(x => x.QuyDoiDonVis)
                .Where(x => x.MaSPCode == code)
                .Select(x => new SanPhamResponse
                {
                    MaSP = x.MaSP,
                    MaSPCode = x.MaSPCode,
                    TenSP = x.TenSP,
                    LoaiSanPham = x.LoaiSanPham.ToString(),
                    DonViCoSo = x.DonViCoSo,
                    GhiChu = x.GhiChu,
                    NgayTao = x.NgayTao,
                    TrangThai = x.TrangThai.ToString(),
                    DonGia = _context.GiaBans
                        .Where(g => g.MaSP == x.MaSP && g.DenNgay == null)
                        .OrderByDescending(g => g.TuNgay)
                        .Select(g => (decimal?)g.DonGiaBan)
                        .FirstOrDefault(),
                    DonViQuyDoi = x.QuyDoiDonVis
                        .Select(q => new DonViQuyDoiItem
                        {
                            DonViNhap = q.DonViNhap ?? string.Empty,
                            TyLe = q.TyLe
                        }).ToList()
                })
                .FirstOrDefaultAsync();
        }

    }
}
