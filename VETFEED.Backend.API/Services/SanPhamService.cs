using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.Common;
using VETFEED.Backend.API.DTOs.SanPham;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Repositories;
using VETFEED.Backend.API.Utils;

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
            if (!Enum.TryParse<LoaiSanPhamEnum>(request.LoaiSanPham, true, out var loai))
                throw new ArgumentException("LoaiSanPham không hợp lệ. Chỉ nhận: THUOC_THU_Y hoặc THUC_AN_CHAN_NUOI.");

            var entity = new SanPham
            {
                MaSP = Guid.NewGuid(),
                MaSPCode = await CodeGenerator.GenerateSanPhamCodeAsync(_context),
                TenSP = request.TenSP.Trim(),
                LoaiSanPham = loai,
                DonViTinh = request.DonViTinh,
                GhiChu = request.GhiChu,
                NgayTao = DateTime.Now
            };

            return await _repo.CreateAsync(entity);
        }

        public async Task<SanPhamResponse?> UpdateAsync(Guid maSP, SanPhamUpdateRequest request)
        {
            if (!Enum.TryParse<LoaiSanPhamEnum>(request.LoaiSanPham, true, out var loai))
                throw new ArgumentException("LoaiSanPham không hợp lệ. Chỉ nhận: THUOC_THU_Y hoặc THUC_AN_CHAN_NUOI.");

            // update các field cơ bản trong repo
            var updated = await _repo.UpdateAsync(maSP, request);
            if (updated == null) return null;

            // set enum trực tiếp (do repo không parse enum)
            var spEntity = await _context.SanPhams.FindAsync(maSP);
            if (spEntity != null)
            {
                spEntity.LoaiSanPham = loai;
                await _context.SaveChangesAsync();
            }

            return await _repo.GetByIdAsync(maSP);
        }

        public async Task<(bool ok, string? error)> DeleteAsync(Guid maSP)
        {
            var exists = await _repo.GetByIdAsync(maSP);
            if (exists == null) return (false, "Không tìm thấy sản phẩm.");

            if (await _repo.HasReferencesAsync(maSP))
                return (false, "Không thể xóa sản phẩm vì đã phát sinh dữ liệu liên quan (Giá bán / Lô hàng / Nhà cung cấp sản phẩm).");

            var ok = await _repo.DeleteAsync(maSP);
            return ok ? (true, null) : (false, "Xóa thất bại.");
        }
    }
}
