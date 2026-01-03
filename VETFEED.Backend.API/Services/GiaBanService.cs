using VETFEED.Backend.API.DTOs.Common;
using VETFEED.Backend.API.DTOs.GiaBan;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class GiaBanService : IGiaBanService
    {
        private readonly IGiaBanRepository _repo;

        public GiaBanService(IGiaBanRepository repo)
        {
            _repo = repo;
        }

        private static DateTime NormalizeStart(DateTime d) => d.Date;
        private static DateTime? NormalizeEnd(DateTime? d) =>
            d.HasValue ? d.Value.Date.AddDays(1).AddTicks(-1) : null;

        public Task<PagedResult<GiaBanResponse>> SearchAsync(GiaBanQuery query) => _repo.SearchAsync(query);

        public Task<GiaBanResponse?> GetByIdAsync(Guid maGia) => _repo.GetByIdAsync(maGia);

        public async Task<GiaBanResponse> CreateAsync(GiaBanCreateRequest request)
        {
            if (!await _repo.ExistsSanPhamAsync(request.MaSP))
                throw new ArgumentException("Không tìm thấy sản phẩm để tạo giá.");

            var tu = NormalizeStart(request.TuNgay);
            var den = NormalizeEnd(request.DenNgay);

            if (den.HasValue && den.Value < tu)
                throw new ArgumentException("DenNgay phải >= TuNgay.");

            if (await _repo.IsOverlappingAsync(request.MaSP, tu, den))
                throw new ArgumentException("Khoảng thời gian giá bán bị trùng (overlap) với một giá khác của sản phẩm.");

            var entity = new GiaBan
            {
                MaGia = Guid.NewGuid(),
                MaSP = request.MaSP,
                DonGiaBan = request.DonGiaBan,
                TuNgay = tu,
                DenNgay = den,
                GhiChu = request.GhiChu,
                NgayTao = DateTime.Now
            };

            return await _repo.CreateAsync(entity);
        }

        public async Task<GiaBanResponse?> UpdateAsync(Guid maGia, GiaBanUpdateRequest request)
        {
            var old = await _repo.GetByIdAsync(maGia);
            if (old == null) return null;

            var tu = request.TuNgay.Date;
            var den = request.DenNgay.HasValue
                ? request.DenNgay.Value.Date.AddDays(1).AddTicks(-1)
                : (DateTime?)null;

            if (den.HasValue && den.Value < tu)
                throw new ArgumentException("DenNgay phải >= TuNgay.");

            // check overlap inclusive
            if (await _repo.IsOverlappingAsync(old.MaSP, tu, den, excludeMaGia: maGia))
                throw new ArgumentException("Khoảng thời gian giá bán bị trùng (overlap) với một giá khác của sản phẩm.");

            // gán lại request sau khi normalize để repo save đúng
            request.TuNgay = tu;
            request.DenNgay = den;

            return await _repo.UpdateAsync(maGia, request);
        }


        public async Task<(bool ok, string? error)> DeleteAsync(Guid maGia)
        {
            var exists = await _repo.GetByIdAsync(maGia);
            if (exists == null) return (false, "Không tìm thấy giá bán.");

            var ok = await _repo.DeleteAsync(maGia);
            return ok ? (true, null) : (false, "Xóa thất bại.");
        }
            public Task<GiaBanResponse?> GetCurrentPriceAsync(Guid maSP, DateTime date)
        => _repo.GetCurrentPriceAsync(maSP, date);

    }
}
