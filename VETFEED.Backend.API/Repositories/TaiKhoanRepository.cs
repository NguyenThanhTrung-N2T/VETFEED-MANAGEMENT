using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.TaiKhoan;
using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public class TaiKhoanRepository : ITaiKhoanRepository
    {
        private readonly VetFeedManagementContext _context;
        public TaiKhoanRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        // kiem tra ton tai so dien thoai
        public async Task<bool> IsSoDienThoaiExistAsync(string sdt)
        {
            return await _context.TaiKhoans.AnyAsync(tk => tk.SoDienThoai == sdt);
        }

        // kiem tra ton tai email 
        public async Task<bool> IsEmailExistAsync(string email)
        {
            return await _context.TaiKhoans.AnyAsync(tk => tk.Email == email);
        }

        // lay tai khoan theo ma tai khoan
        public async Task<TaiKhoanResponse?> GetTaiKhoanByIdAsync(Guid maTK)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(maTK);
            if(taiKhoan == null)
            {
                return null;
            }
            return MapToResponse(taiKhoan);
        }

        // tao tai khoan
        public async Task<TaiKhoanResponse> CreateTaiKhoanAsync(CreateTaiKhoanRequest request)
        {
            try
            {
                // tai khoan 
                var taiKhoan = new TaiKhoan
                {
                    HoTen = request.HoTen,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
                    Role = Enums.RoleEnum.NHAN_VIEN,
                    TrangThai = Enums.TrangThaiTaiKhoanEnum.HOAT_DONG
                };

                // luu vao db 
                _context.TaiKhoans.Add(taiKhoan);
                await _context.SaveChangesAsync();
                return MapToResponse(taiKhoan);
                
            } catch (Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi đăng ký tài khoản !", ex);
            }
        }


        private TaiKhoanResponse MapToResponse(TaiKhoan taiKhoan)
        {
            return new TaiKhoanResponse
            {
                MaTK = taiKhoan.MaTK,
                Email = taiKhoan.Email,
                HoTen = taiKhoan.HoTen,
                SoDienThoai = taiKhoan.SoDienThoai,
                AnhDaiDien = taiKhoan.AnhDaiDien,
                Role = taiKhoan.Role,             // Enum → giữ nguyên
                TrangThai = taiKhoan.TrangThai,   // Enum → giữ nguyên
            };
        }

    }
}
