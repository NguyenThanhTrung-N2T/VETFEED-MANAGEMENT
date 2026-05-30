using VETFEED.Backend.API.DTOs.PhieuBan;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class PhieuBanService : IPhieuBanService
    {
        private readonly IPhieuBanRepository _phieuBanRepo;

        public PhieuBanService(IPhieuBanRepository phieuBanRepo)
        {
            _phieuBanRepo = phieuBanRepo;
        }

        // lay danh sach phieu ban
        public async Task<IEnumerable<PhieuBanResponse>> GetDanhSachPhieuBanAsync()
        {
            return await _phieuBanRepo.GetDanhSachPhieuBanAsync();
        }

        // lay chi tiet phieu ban
        public async Task<PhieuBanDetailResponse?> GetChiTietPhieuBanAsync(Guid maPB)
        {
            return await _phieuBanRepo.GetChiTietPhieuBanAsync(maPB);
        }

        // Tao phieu ban
        public async Task<(PhieuBanDetailResponse? result, string? error)> CreatePhieuBanAsync(CreatePhieuBanRequest request)
        {
            try
            {
                // kiem tra dau vao
                if (request.DanhSachChiTiet == null || request.DanhSachChiTiet.Count == 0)
                    return (null, "Danh sách chi tiết không được để trống !");

                if (request.TienCoc < 0)
                    return (null, "Tiền cọc không được âm !");

                if (request.ChietKhauPhanTram < 0 || request.ChietKhauPhanTram > 100)
                    return (null, "Chiết khấu phải từ 0 đến 100 % !");

                //tao phieu ban
                var result = await _phieuBanRepo.CreatePhieuBanAsync(request);
                return (result, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }


        // Xoa phieu ban 
        public async Task<bool> DeletePhieuBanAsync(Guid maPB)
        {
            try
            {
                return await _phieuBanRepo.DeletePhieuBanAsync(maPB);
            }
            catch (Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi xóa phiếu bán !", ex);
            }
        }

        // lay lich su mua hang cua khach hang
        public async Task<KhachHangPhieuBanResponse> GetPhieuBanTheoKhachHangAsync(Guid maKH)
        {
            if (maKH == Guid.Empty)
                throw new ArgumentException("Mã khách hàng không hợp lệ");

            var result = await _phieuBanRepo.GetPhieuBanByKhachHangAsync(maKH);

            if (result == null)
                throw new KeyNotFoundException("Không tìm thấy khách hàng");

            return result;
        }
    }
}
