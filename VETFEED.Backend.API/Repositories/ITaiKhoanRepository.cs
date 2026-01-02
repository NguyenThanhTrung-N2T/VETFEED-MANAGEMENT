using VETFEED.Backend.API.DTOs.TaiKhoan;

namespace VETFEED.Backend.API.Repositories
{
    public interface ITaiKhoanRepository
    {
        // kiểm tra số điện thoại tồn tại
        Task<bool> IsSoDienThoaiExistAsync(string sdt);
        // kiểm tra email tồn tại 
        Task<bool> IsEmailExistAsync(string email);

        // tao tai khoan 
        Task<TaiKhoanResponse> CreateTaiKhoanAsync(CreateTaiKhoanRequest request);

        // lay tai khoan theo ma tai khoan 
        Task<TaiKhoanResponse?> GetTaiKhoanByIdAsync(Guid maTK); 
    }
}
