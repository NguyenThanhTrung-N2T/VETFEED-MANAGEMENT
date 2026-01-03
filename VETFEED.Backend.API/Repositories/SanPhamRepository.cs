using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.Common;
using VETFEED.Backend.API.DTOs.SanPham;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Enums;
using System;

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
            var q = _context.SanPhams.AsNoTracking().AsQueryable();

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

            if (query.Page.HasValue && query.PageSize.HasValue && query.Page > 0 && query.PageSize > 0)
            {
                q = q.OrderByDescending(x => x.NgayTao)
                     .Skip((query.Page.Value - 1) * query.PageSize.Value)
                     .Take(query.PageSize.Value);
            }
            else
            {
                q = q.OrderByDescending(x => x.NgayTao);
            }

            var items = await q.Select(x => new SanPhamResponse
            {
                MaSP = x.MaSP,
                MaSPCode = x.MaSPCode,
                TenSP = x.TenSP,
                LoaiSanPham = x.LoaiSanPham.ToString(),
                DonViTinh = x.DonViTinh,
                GhiChu = x.GhiChu,
                NgayTao = x.NgayTao
            }).ToListAsync();

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
                .Where(x => x.MaSP == maSP)
                .Select(x => new SanPhamResponse
                {
                    MaSP = x.MaSP,
                    MaSPCode = x.MaSPCode,
                    TenSP = x.TenSP,
                    LoaiSanPham = x.LoaiSanPham.ToString(),
                    DonViTinh = x.DonViTinh,
                    GhiChu = x.GhiChu,
                    NgayTao = x.NgayTao
                }).FirstOrDefaultAsync();
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
            sp.DonViTinh = request.DonViTinh;
            sp.GhiChu = request.GhiChu;
            // LoaiSanPham sẽ set ở service (parse enum)
            await _context.SaveChangesAsync();

            return await GetByIdAsync(maSP);
        }

        public async Task<bool> DeleteAsync(Guid maSP)
        {
            var sp = await _context.SanPhams.FirstOrDefaultAsync(x => x.MaSP == maSP);
            if (sp == null) return false;

            _context.SanPhams.Remove(sp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasReferencesAsync(Guid maSP)
        {
            var hasGia = await _context.GiaBans.AnyAsync(x => x.MaSP == maSP);
            if (hasGia) return true;

            var hasLo = await _context.LoHangs.AnyAsync(x => x.MaSP == maSP);
            if (hasLo) return true;

            var hasNcsp = await _context.NhaCungCapSanPhams.AnyAsync(x => x.MaSP == maSP);
            return hasNcsp;
        }
    }
}
