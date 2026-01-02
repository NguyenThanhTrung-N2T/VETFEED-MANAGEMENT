using VETFEED.Backend.API.DTOs.TaiKhoan;

namespace VETFEED.Backend.API.Services
{
    public interface ITaiKhoanService
    {
        // tao tai khoan
        Task<TaiKhoanResponse?> CreatTaiKhoanAsync(CreateTaiKhoanRequest request);

        // lay tai khoan theo ma tai khoan 
        Task<TaiKhoanResponse?> GetTaiKhoanByIdAsync(Guid maTK);
    }
}
