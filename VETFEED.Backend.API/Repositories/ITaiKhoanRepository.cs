using VETFEED.Backend.API.DTOs.TaiKhoan;
using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public interface ITaiKhoanRepository
    {
        // kiểm tra số điện thoại tồn tại
        Task<bool> IsSoDienThoaiExistAsync(string sdt);
        // kiểm tra email tồn tại 
        Task<bool> IsEmailExistAsync(string email);

        // kiểm tra email tồn tại (không tính tài khoản hiện tại)
        Task<bool> IsEmailExistAsync(string email, Guid excludeMaTK);
        // kiểm tra số điện thoại tồn tại (không tính tài khoản hiện tại)
        Task<bool> IsSoDienThoaiExistAsync(string sdt, Guid excludeMaTK);

        // tao tai khoan 
        Task<TaiKhoanResponse> CreateTaiKhoanAsync(CreateTaiKhoanRequest request);

        // lay tai khoan theo ma tai khoan 
        Task<TaiKhoanResponse?> GetTaiKhoanByIdAsync(Guid maTK);

        // cap nhat tai khoan
        Task<TaiKhoanResponse?> UpdateTaiKhoanAsync(Guid maTK, UpdateTaiKhoanRequest request);

        // dang nhap tai khoan
        Task<bool> Login(LoginRequest request);

        // lay tai khoan bang email 
        Task<TaiKhoanResponse?> GetTaiKhoanByEmailAsync(string email);

        // cap nhat mat khau 
        Task<bool> UpdatePasswordAsync(string email, string newPass);
    }
}
