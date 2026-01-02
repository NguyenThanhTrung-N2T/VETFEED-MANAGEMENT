using Azure.Core;
using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.KhoHang;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Utils;

namespace VETFEED.Backend.API.Repositories
{
    public class KhoHangRepository : IKhoHangRepository
    {
        private readonly VetFeedManagementContext _context;
        public KhoHangRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        // lấy tất cả kho hàng
        public async Task<IEnumerable<KhoHangResponse>> GetAllKhoHangsAsync()
        {
            // lấy tất cả kho hàng
            var khoHangs = await _context.KhoHangs.ToListAsync();
            // trả về danh sách
            return khoHangs.Select(MapToResponse);
        }

        // lấy kho hàng theo mã kho
        public async Task<KhoHangResponse?> GetKhoHangByIdAsync(Guid maKho)
        {
            // lấy thông tin kho hàng 
            var khoHang = await _context.KhoHangs.FindAsync(maKho);
            if(khoHang == null)
            {
                return null;
            }
            // trả về kho hàng 
            return MapToResponse(khoHang);
        }

        // thêm kho hàng
        public async Task<KhoHangResponse> AddKhoHangAsync(CreateKhoHangRequest dto)
        {
            try
            {
                // tạo kho hàng
                var khoHang = new KhoHang
                {
                    MaKhoCode = await CodeGenerator.GenerateKhoHangCodeAsync(_context),
                    TenKho = dto.TenKho,
                    DiaChi = dto.DiaChi,
                    TrangThai = dto.TrangThai,
                    GhiChu = dto.GhiChu
                };

                // thêm vào db 
                _context.KhoHangs.Add(khoHang);
                await _context.SaveChangesAsync();

                // trả về kho hàng
                return MapToResponse(khoHang);

            } catch (Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi thêm kho hàng !", ex);
            }
        }

        // Tìm kho hàng theo mã kho
        public async Task<bool> IsKhoHangExist(Guid maKho)
        {
            return await _context.KhoHangs.AnyAsync(k => k.MaKho == maKho);
        }

        // Cập nhật thông tin kho hàng 
        public async Task<KhoHangResponse> UpdateKhoHangAsync(Guid maKho, UpdateKhoHangRequest dto)
        {
            try
            {
                // lất kho hàng trong db
                var khoHang = await _context.KhoHangs.FindAsync(maKho);
                if(khoHang == null)
                {
                    return null!;
                }

                // cập nhật thông tin
                khoHang.TenKho = dto.TenKho;
                khoHang.DiaChi = dto.DiaChi;
                khoHang.GhiChu = dto.GhiChu;
                khoHang.TrangThai = dto.TrangThai;

                // lưu thay đổi 
                await _context.SaveChangesAsync();

                // trả về kho hàng sau cập nhật 
                return MapToResponse(khoHang);

            }catch (Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi cập nhật thông tin kho hàng !", ex);
            }
        }

        // Xóa kho hàng 
        public async Task<bool> DeleteKhoHangAsync(Guid maKho)
        {
            // lấy kho hàng trong db
            var khoHang = await _context.KhoHangs.FindAsync(maKho);
            if(khoHang == null)
            {
                return false;
            }

            // Xóa kho hàng 
            _context.KhoHangs.Remove(khoHang);
            await _context.SaveChangesAsync();
            return true;
        }

        // tìm kiếm theo thuộc tính và từ khóa
        public async Task<IEnumerable<KhoHangResponse>> SearchKhoHangAsync(SearchKhoHangRequest dto)
        {
            var query = _context.KhoHangs.AsQueryable();

            // Không có thuộc tính
            if (string.IsNullOrWhiteSpace(dto.ThuocTinh))
            {
                if (string.IsNullOrWhiteSpace(dto.KeyWord))
                {
                    // Không có thuộc tính lẫn từ khóa → trả về tất cả
                    var result = await query.ToListAsync(); 
                    return result.Select(k => MapToResponse(k)).ToList();
                }
                else
                {
                    // Không có thuộc tính nhưng có từ khóa → tìm trong tất cả thuộc tính
                    query = query.Where(k =>
                        k.TenKho!.Contains(dto.KeyWord) ||
                        k.DiaChi!.Contains(dto.KeyWord) ||
                        k.GhiChu!.Contains(dto.KeyWord)
                    );
                }
            }
            else
            {
                // Có thuộc tính
                if (string.IsNullOrWhiteSpace(dto.KeyWord))
                {
                    // Có thuộc tính nhưng không có từ khóa → không lọc gì, trả về tất cả
                    var result = await query.ToListAsync(); 
                    return result.Select(k => MapToResponse(k)).ToList();
                }
                else
                {
                    // Có cả thuộc tính và từ khóa → lọc theo đúng field
                    switch (dto.ThuocTinh.ToLower())
                    {
                        case "ten kho":
                            query = query.Where(k => k.TenKho!.Contains(dto.KeyWord));
                            break;
                        case "dia chi":
                            query = query.Where(k => k.DiaChi!.Contains(dto.KeyWord));
                            break;
                        case "ghi chu":
                            query = query.Where(k => k.GhiChu!.Contains(dto.KeyWord));
                            break;
                        default:
                            // Nếu thuộc tính không hợp lệ thì không lọc gì
                            break;
                    }
                }
            }

            // trả về danh sách 
            var data = await query.ToListAsync(); 
            return data.Select(k => MapToResponse(k)).ToList();
        }


        private KhoHangResponse MapToResponse(KhoHang khoHang) 
        {
            return new KhoHangResponse 
            { 
                MaKho = khoHang.MaKho, 
                MaKhoCode = khoHang.MaKhoCode, 
                TenKho = khoHang.TenKho, 
                DiaChi = khoHang.DiaChi, 
                TrangThai = khoHang.TrangThai.ToString(), 
                GhiChu = khoHang.GhiChu 
            }; 
        }
    }
}
