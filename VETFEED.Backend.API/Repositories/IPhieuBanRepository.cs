using VETFEED.Backend.API.DTOs.PhieuBan;

namespace VETFEED.Backend.API.Repositories
{
    public interface IPhieuBanRepository
    {
        // lay danh sach phieu ban
        Task<IEnumerable<PhieuBanResponse>> GetDanhSachPhieuBanAsync();

        // lay chi tiet phieu ban
        Task<PhieuBanDetailResponse?> GetChiTietPhieuBanAsync(Guid maPB);

        // tao phieu ban
        Task<PhieuBanDetailResponse> CreatePhieuBanAsync(CreatePhieuBanRequest request);


        // Xóa phiếu bán
        Task<bool> DeletePhieuBanAsync(Guid maPB);

        // lay lich su mua hang cua khach hang
        Task<KhachHangPhieuBanResponse?> GetPhieuBanByKhachHangAsync(Guid maKH);
    }
}
