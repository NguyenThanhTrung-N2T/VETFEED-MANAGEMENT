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

        // kiem tra ton tai email (loai tru tai khoan hien tai)
        public async Task<bool> IsEmailExistAsync(string email, Guid excludeMaTK)
        {
            return await _context.TaiKhoans.AnyAsync(tk => tk.Email == email && tk.MaTK != excludeMaTK);
        }

        // kiem tra ton tai so dien thoai (loai tru tai khoan hien tai)
        public async Task<bool> IsSoDienThoaiExistAsync(string sdt, Guid excludeMaTK)
        {
            return await _context.TaiKhoans.AnyAsync(tk => tk.SoDienThoai == sdt && tk.MaTK != excludeMaTK);
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

        // cap nhat tai khoan
        public async Task<TaiKhoanResponse?> UpdateTaiKhoanAsync(Guid maTK, UpdateTaiKhoanRequest request)
        {
            try
            {
                var taiKhoan = await _context.TaiKhoans.FindAsync(maTK);
                if (taiKhoan == null)
                {
                    return null;
                }

                // cap nhat cac truong neu co du lieu
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    taiKhoan.Email = request.Email;
                }

                if (!string.IsNullOrWhiteSpace(request.HoTen))
                {
                    taiKhoan.HoTen = request.HoTen;
                }

                if (!string.IsNullOrWhiteSpace(request.SoDienThoai))
                {
                    taiKhoan.SoDienThoai = request.SoDienThoai;
                }

                if (!string.IsNullOrWhiteSpace(request.AnhDaiDien))
                {
                    taiKhoan.AnhDaiDien = request.AnhDaiDien;
                }

                // luu thay doi
                _context.TaiKhoans.Update(taiKhoan);
                await _context.SaveChangesAsync();

                return MapToResponse(taiKhoan);

            } catch (Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi cập nhật tài khoản !", ex);
            }
        }

        // dang nhap tai khoan 
        public async Task<bool> Login(LoginRequest request)
        {
            // lay tai khoan 
            var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(tk => tk.Email == request.Email);
            if(taiKhoan == null)
            {
                return false;
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, taiKhoan.PasswordHash);
            return isValidPassword;
        }

        public async Task<TaiKhoanResponse?> GetTaiKhoanByEmailAsync(string email)
        {
            var taikhoan = await _context.TaiKhoans.FirstOrDefaultAsync(tk => tk.Email == email);
            if (taikhoan == null)
            {
                return null;
            }
            return MapToResponse(taikhoan);
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
