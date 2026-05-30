using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.QuyDoiDonVi;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class QuyDoiDonViService : IQuyDoiDonViService
    {
        private readonly IQuyDoiDonViRepository _repo;
        private readonly VetFeedManagementContext _context;

        public QuyDoiDonViService(IQuyDoiDonViRepository repo, VetFeedManagementContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<IEnumerable<QuyDoiDonViResponse>> GetByMaSPAsync(Guid maSP)
        {
            var sp = await _context.SanPhams.AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaSP == maSP);

            if (sp == null)
                throw new ArgumentException("Không tìm thấy sản phẩm.");

            var list = await _repo.GetByMaSPAsync(maSP);

            return list.Select(q => new QuyDoiDonViResponse
            {
                MaQD = q.MaQD,
                MaSP = q.MaSP,
                MaSPCode = sp.MaSPCode,
                TenSP = sp.TenSP,
                DonViNhap = q.DonViNhap ?? string.Empty,
                TyLe = q.TyLe
            });
        }

        public async Task<QuyDoiDonViResponse> CreateAsync(Guid maSP, QuyDoiDonViCreateRequest request)
        {
            var sp = await _context.SanPhams.AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaSP == maSP);

            if (sp == null)
                throw new ArgumentException("Không tìm thấy sản phẩm.");

            if (string.IsNullOrWhiteSpace(request.DonViNhap))
                throw new ArgumentException("Đơn vị nhập không được để trống.");

            var donViNhap = request.DonViNhap.Trim().ToUpper();

            if (await _repo.ExistsDonViNhapAsync(maSP, donViNhap, null))
                throw new ArgumentException("Đơn vị nhập này đã được cấu hình cho sản phẩm.");

            var entity = new QuyDoiDonVi
            {
                MaQD = Guid.NewGuid(),
                MaSP = maSP,
                DonViNhap = donViNhap,
                TyLe = request.TyLe
            };

            var created = await _repo.AddAsync(entity);

            return new QuyDoiDonViResponse
            {
                MaQD = created.MaQD,
                MaSP = created.MaSP,
                MaSPCode = sp.MaSPCode,
                TenSP = sp.TenSP,
                DonViNhap = created.DonViNhap ?? string.Empty,
                TyLe = created.TyLe
            };
        }

        public async Task<QuyDoiDonViResponse?> UpdateAsync(Guid maQD, QuyDoiDonViUpdateRequest request)
        {
            var existing = await _repo.GetByIdAsync(maQD);
            if (existing == null) return null;

            if (string.IsNullOrWhiteSpace(request.DonViNhap))
                throw new ArgumentException("Đơn vị nhập không được để trống.");

            var donViNhap = request.DonViNhap.Trim().ToUpper();

            if (await _repo.ExistsDonViNhapAsync(existing.MaSP, donViNhap, maQD))
                throw new ArgumentException("Đơn vị nhập này đã được cấu hình cho sản phẩm.");

            var entity = existing;
            entity.DonViNhap = donViNhap;
            entity.TyLe = request.TyLe;

            var updated = await _repo.UpdateAsync(entity);
            if (updated == null) return null;

            var sp = await _context.SanPhams.AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaSP == entity.MaSP);

            return new QuyDoiDonViResponse
            {
                MaQD = updated.MaQD,
                MaSP = updated.MaSP,
                MaSPCode = sp?.MaSPCode,
                TenSP = sp?.TenSP,
                DonViNhap = updated.DonViNhap ?? string.Empty,
                TyLe = updated.TyLe
            };
        }



        public Task<bool> DeleteAsync(Guid maQD) => _repo.DeleteAsync(maQD);

        public async Task<UnitsForProductResponse> GetUnitsForProductAsync(Guid maSP)
        {
            var sp = await _context.SanPhams.AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaSP == maSP);

            if (sp == null)
                throw new ArgumentException("Không tìm thấy sản phẩm.");

            var list = await _repo.GetByMaSPAsync(maSP);

            return new UnitsForProductResponse
            {
                MaSP = sp.MaSP,
                MaSPCode = sp.MaSPCode,
                TenSP = sp.TenSP,
                DonViCoSo = sp.DonViCoSo,
                DonViNhapList = list.Select(q => new DonViQuyDoiItem
                {
                    DonViNhap = q.DonViNhap ?? string.Empty,
                    TyLe = q.TyLe
                }).ToList()
            };
        }
    }
}
