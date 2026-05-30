using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.Common;
using VETFEED.Backend.API.DTOs.SanPham;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Repositories;
using VETFEED.Backend.API.Utils;
using Microsoft.EntityFrameworkCore;
namespace VETFEED.Backend.API.Services
{
    public class SanPhamService : ISanPhamService
    {
        private readonly ISanPhamRepository _repo;
        private readonly VetFeedManagementContext _context;

        public SanPhamService(ISanPhamRepository repo, VetFeedManagementContext context)
        {
            _repo = repo;
            _context = context;
        }

        public Task<PagedResult<SanPhamResponse>> SearchAsync(SanPhamQuery query) => _repo.SearchAsync(query);

        public Task<SanPhamResponse?> GetByIdAsync(Guid maSP) => _repo.GetByIdAsync(maSP);

        public async Task<SanPhamResponse> CreateAsync(SanPhamCreateRequest request)
        {
            if (!Enum.TryParse<LoaiSanPhamEnum>(request.LoaiSanPham.Trim(), true, out var loai))
                throw new ArgumentException("LoaiSanPham không hợp lệ. Chỉ nhận: THUOC_THU_Y hoặc THUC_AN_CHAN_NUOI.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            var now = DateTime.Now;

            var entity = new SanPham
            {
                MaSP = Guid.NewGuid(),
                MaSPCode = await CodeGenerator.GenerateSanPhamCodeAsync(_context),
                TenSP = request.TenSP.Trim(),
                LoaiSanPham = loai,
                DonViCoSo = request.DonViTinh,
                AnhSanPham = request.AnhSanPham,
                GhiChu = request.GhiChu,
                NgayTao = now,
                TrangThai = TrangThaiSanPhamEnum.HoatDong
            };

            var created = await _repo.CreateAsync(entity);
            if (request.GiaBanDau.HasValue)
            {
                var giaBan = new GiaBan
                {
                    MaGia = Guid.NewGuid(),
                    MaSP = entity.MaSP,
                    DonGiaBan = request.GiaBanDau.Value,
                    TuNgay = now,
                    DenNgay = null,
                    NgayTao = now,
                    GhiChu = "Giá khởi tạo khi tạo sản phẩm"
                };

                _context.GiaBans.Add(giaBan);
                await _context.SaveChangesAsync();
            }

            // NEW: tạo các dòng QuyDoiDonVi
            if (request.DonViQuyDoi != null && request.DonViQuyDoi.Any())
            {
                foreach (var dv in request.DonViQuyDoi)
                {
                    if (string.IsNullOrWhiteSpace(dv.DonViNhap)) continue;

                    var unit = new QuyDoiDonVi
                    {
                        MaQD = Guid.NewGuid(),
                        MaSP = entity.MaSP,
                        DonViNhap = dv.DonViNhap.Trim(),
                        TyLe = dv.TyLe
                    };
                    _context.QuyDoiDonVis.Add(unit);
                }

                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();

            return (await _repo.GetByIdAsync(entity.MaSP))!;
        }

        public async Task<SanPhamResponse?> UpdateAsync(Guid maSP, SanPhamUpdateRequest request)
        {
            if (!Enum.TryParse<LoaiSanPhamEnum>(request.LoaiSanPham.Trim(), true, out var loai))
                throw new ArgumentException("LoaiSanPham không hợp lệ. Chỉ nhận: THUOC_THU_Y hoặc THUC_AN_CHAN_NUOI.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            var updated = await _repo.UpdateAsync(maSP, request);
            if (updated == null)
                return null;

            var spEntity = await _context.SanPhams.FindAsync(maSP);
            if (spEntity != null)
            {
                spEntity.LoaiSanPham = loai;
                spEntity.DonViCoSo = request.DonViTinh;
                spEntity.AnhSanPham = request.AnhSanPham;

                await _context.SaveChangesAsync();
            }

            var oldUnits = await _context.QuyDoiDonVis
                .Where(u => u.MaSP == maSP)
                .ToListAsync();

            if (oldUnits.Any())
            {
                _context.QuyDoiDonVis.RemoveRange(oldUnits);
                await _context.SaveChangesAsync();
            }

            if (request.DonViQuyDoi != null && request.DonViQuyDoi.Any())
            {
                foreach (var dv in request.DonViQuyDoi)
                {
                    if (string.IsNullOrWhiteSpace(dv.DonViNhap)) continue;

                    var unit = new QuyDoiDonVi
                    {
                        MaQD = Guid.NewGuid(),
                        MaSP = maSP,
                        DonViNhap = dv.DonViNhap.Trim(),
                        TyLe = dv.TyLe
                    };
                    _context.QuyDoiDonVis.Add(unit);
                }

                await _context.SaveChangesAsync();
            }

            if (request.GiaMoi.HasValue)
            {
                var giaMoi = request.GiaMoi.Value;

                var giaHienTai = await _context.GiaBans
                    .Where(g => g.MaSP == maSP && g.DenNgay == null)
                    .OrderByDescending(g => g.TuNgay)
                    .FirstOrDefaultAsync();

                var giaHienTaiValue = giaHienTai?.DonGiaBan;

                if (!giaHienTaiValue.HasValue || giaHienTaiValue.Value != giaMoi)
                {
                    if (giaHienTai != null)
                    {
                        giaHienTai.DenNgay = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }

                    var giaBanMoi = new GiaBan
                    {
                        MaGia = Guid.NewGuid(),
                        MaSP = maSP,
                        DonGiaBan = giaMoi,
                        TuNgay = DateTime.Now,
                        DenNgay = null,
                        NgayTao = DateTime.Now,
                        GhiChu = "Cập nhật giá từ màn hình sản phẩm"
                    };

                    _context.GiaBans.Add(giaBanMoi);
                    await _context.SaveChangesAsync();
                }
            }

            await transaction.CommitAsync();

            return await _repo.GetByIdAsync(maSP);
        }

        public async Task<(bool ok, string? error)> DeleteAsync(Guid maSP)
        {
            var exists = await _repo.GetByIdAsync(maSP);
            if (exists == null) return (false, "Không tìm thấy sản phẩm.");

            if (await _repo.HasReferencesAsync(maSP))
                return (false, "Không thể xóa sản phẩm vì đã phát sinh dữ liệu liên quan (Lô hàng / Nhà cung cấp sản phẩm).");

            using var transaction = await _context.Database.BeginTransactionAsync();

            var units = await _context.QuyDoiDonVis
                .Where(u => u.MaSP == maSP)
                .ToListAsync();

            if (units.Any())
            {
                _context.QuyDoiDonVis.RemoveRange(units);
                await _context.SaveChangesAsync();
            }

            var ok = await _repo.DeleteAsync(maSP);

            if (!ok)
            {
                await transaction.RollbackAsync();
                return (false, "Xóa thất bại.");
            }

            await transaction.CommitAsync();
            return (true, null);
        }

        public Task<SanPhamResponse?> GetByCodeAsync(string maSPCode)
        {
            if (string.IsNullOrWhiteSpace(maSPCode))
                return Task.FromResult<SanPhamResponse?>(null);

            return _repo.GetByCodeAsync(maSPCode);
        }

    }
}
