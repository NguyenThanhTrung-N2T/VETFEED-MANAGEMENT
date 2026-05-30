using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.Common;
using VETFEED.Backend.API.DTOs.KhachHang;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Repositories;
using VETFEED.Backend.API.Utils;

namespace VETFEED.Backend.API.Services
{
    public class KhachHangService : IKhachHangService
    {
        private readonly IKhachHangRepository _repo;
        private readonly VetFeedManagementContext _context;

        public KhachHangService(IKhachHangRepository repo, VetFeedManagementContext context)
        {
            _repo = repo;
            _context = context;
        }

        public Task<PagedResult<KhachHangResponse>> SearchAsync(KhachHangQuery query) => _repo.SearchAsync(query);

        public Task<KhachHangResponse?> GetByIdAsync(Guid maKH) => _repo.GetByIdAsync(maKH);

        public async Task<KhachHangResponse> CreateAsync(KhachHangCreateRequest request)
        {
            var loai = request.LoaiKhachHang;
            var trangThai = request.TrangThai;

            if (!string.IsNullOrWhiteSpace(request.SoDienThoai))
            {
                var phone = request.SoDienThoai.Trim();
                if (await _repo.PhoneExistsAsync(phone))
                    throw new ArgumentException("Số điện thoại đã tồn tại.");
            }

            var entity = new KhachHang
            {
                MaKH = Guid.NewGuid(),
                MaKHCode = await CodeGenerator.GenerateKhachHangCodeAsync(_context),
                TenKH = request.TenKH.Trim(),
                SoDienThoai = request.SoDienThoai?.Trim(),
                DiaChi = request.DiaChi,
                LoaiKhachHang = loai,
                HanMucCongNo = request.HanMucCongNo,
                TongMua = 0,
                CongNoHienTai = 0,
                TrangThai = trangThai,
                GhiChu = request.GhiChu,
                NgayTao = DateTime.Now
            };

            return await _repo.CreateAsync(entity);
        }

        public async Task<KhachHangResponse?> UpdateAsync(Guid maKH, KhachHangUpdateRequest request)
        {
            var loai = request.LoaiKhachHang;
            var trangThai = request.TrangThai;

            if (!string.IsNullOrWhiteSpace(request.SoDienThoai))
            {
                var phone = request.SoDienThoai.Trim();
                if (await _repo.PhoneExistsAsync(phone, maKH))
                    throw new ArgumentException("Số điện thoại đã tồn tại.");
            }

            // update basic fields
            var updated = await _repo.UpdateAsync(maKH, request);
            if (updated == null) return null;

            var entity = await _context.KhachHangs.FindAsync(maKH);
            if (entity != null)
            {
                entity.LoaiKhachHang = loai;
                entity.TrangThai = trangThai;
                await _context.SaveChangesAsync();
            }

            return await _repo.GetByIdAsync(maKH);
        }

        public async Task<(bool ok, string? error)> DeleteAsync(Guid maKH)
        {
            var exists = await _repo.GetByIdAsync(maKH);
            if (exists == null) return (false, "Không tìm thấy khách hàng.");

            if (await _repo.HasReferencesAsync(maKH))
                return (false, "Không thể xóa khách hàng vì đã phát sinh dữ liệu liên quan (Phiếu bán / Phiếu trả / Công nợ).");

            var ok = await _repo.DeleteAsync(maKH);
            return ok ? (true, null) : (false, "Xóa thất bại.");
        }
        public Task<KhachHangResponse?> GetByCodeAsync(string maKHCode)
        {
            if (string.IsNullOrWhiteSpace(maKHCode))
                return Task.FromResult<KhachHangResponse?>(null);

            return _repo.GetByCodeAsync(maKHCode.Trim());
        }

        public Task<KhachHangResponse?> GetByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return Task.FromResult<KhachHangResponse?>(null);

            return _repo.GetByPhoneAsync(phone.Trim());
        }

    }
}
