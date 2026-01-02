using VETFEED.Backend.API.DTOs.TaiKhoan;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly ITaiKhoanRepository _taiKhoanRepo;
        public TaiKhoanService(ITaiKhoanRepository taiKhoanRepo)
        {
            _taiKhoanRepo = taiKhoanRepo;
        }

        // lay tai khoan theo ma tai khoan 
        public async Task<TaiKhoanResponse?> GetTaiKhoanByIdAsync(Guid maTK)
        {
            return await _taiKhoanRepo.GetTaiKhoanByIdAsync(maTK);
        }

        // tao tai khoan
        public async Task<TaiKhoanResponse?> CreatTaiKhoanAsync(CreateTaiKhoanRequest request)
        {
            try
            {
                // kiem tra email ton tai 
                var isExist = await _taiKhoanRepo.IsEmailExistAsync(request.Email!);
                if (isExist)
                {
                    return null;
                }
                // tao tai khoan
                return await _taiKhoanRepo.CreateTaiKhoanAsync(request);

            } catch (Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi đăng ký tài khoản !", ex);
            }
        }
    }
}
