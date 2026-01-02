using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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

        // cap nhat tai khoan
        public async Task<TaiKhoanResponse?> UpdateTaiKhoanAsync(Guid maTK, UpdateTaiKhoanRequest request)
        {
            try
            {
                // kiem tra tai khoan ton tai
                var existingTaiKhoan = await _taiKhoanRepo.GetTaiKhoanByIdAsync(maTK);
                if (existingTaiKhoan == null)
                {
                    throw new Exception("Tài khoản không tồn tại !");
                }

                // kiem tra email neu co update
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    // neu email co thay doi
                    if (request.Email != existingTaiKhoan.Email)
                    {
                        // kiem tra email da ton tai o tai khoan khac
                        var emailExists = await _taiKhoanRepo.IsEmailExistAsync(request.Email, maTK);
                        if (emailExists)
                        {
                            throw new Exception("Email này đã được sử dụng bởi tài khoản khác !");
                        }
                    }
                }

                // kiem tra so dien thoai neu co update
                if (!string.IsNullOrWhiteSpace(request.SoDienThoai))
                {
                    // neu so dien thoai co thay doi
                    if (request.SoDienThoai != existingTaiKhoan.SoDienThoai)
                    {
                        // kiem tra so dien thoai da ton tai o tai khoan khac
                        var sdtExists = await _taiKhoanRepo.IsSoDienThoaiExistAsync(request.SoDienThoai, maTK);
                        if (sdtExists)
                        {
                            throw new Exception("Số điện thoại này đã được sử dụng bởi tài khoản khác !");
                        }
                    }
                }

                // cap nhat tai khoan
                var updatedTaiKhoan = await _taiKhoanRepo.UpdateTaiKhoanAsync(maTK, request);
                if (updatedTaiKhoan == null)
                {
                    return null;
                }
                return updatedTaiKhoan;

            } catch (Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi cập nhật tài khoản !", ex);
            }
        }
    }
}
