using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;

namespace VETFEED.Backend.API.Utils
{
    /// <summary>
    /// Class sinh mã code random cho các bảng, đảm bảo không trùng trong DB
    /// </summary>
    public static class CodeGenerator
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Sinh mã code random với prefix, độ dài phần random, và check DB
        /// </summary>
        private static async Task<string> GenerateUniqueCodeAsync(
            VetFeedManagementContext context,
            string prefix,
            int randomLength,
            Func<VetFeedManagementContext, string, Task<bool>> existsFunc)
        {
            string newCode;
            do
            {
                // Sinh chuỗi random gồm chữ + số
                newCode = prefix + RandomString(randomLength);
            }
            while (await existsFunc(context, newCode));

            return newCode;
        }

        /// <summary>
        /// Sinh chuỗi random gồm chữ và số
        /// </summary>
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] buffer = new char[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = chars[_random.Next(chars.Length)];
            }
            return new string(buffer);
        }

        // ============================
        // Các hàm public cho từng bảng
        // ============================

        // Khách hàng
        public static Task<string> GenerateKhachHangCodeAsync(VetFeedManagementContext context)
        {
            return GenerateUniqueCodeAsync(context, "KH", 6,
                async (ctx, code) => await ctx.KhachHangs.AnyAsync(kh => kh.MaKHCode == code));
        }

        // Sản phẩm
        public static Task<string> GenerateSanPhamCodeAsync(VetFeedManagementContext context)
        {
            return GenerateUniqueCodeAsync(context, "SP", 6,
                async (ctx, code) => await ctx.SanPhams.AnyAsync(sp => sp.MaSPCode == code));
        }

        // Kho hàng
        public static Task<string> GenerateKhoHangCodeAsync(VetFeedManagementContext context)
        {
            return GenerateUniqueCodeAsync(context, "K", 6,
                async (ctx, code) => await ctx.KhoHangs.AnyAsync(k => k.MaKhoCode == code));
        }

        // Nhà cung cấp
        public static Task<string> GenerateNhaCungCapCodeAsync(VetFeedManagementContext context)
        {
            return GenerateUniqueCodeAsync(context, "NCC", 6,
                async (ctx, code) => await ctx.NhaCungCaps.AnyAsync(ncc => ncc.MaNCCCode == code));
        }

        // Lô hàng
        public static Task<string> GenerateLoHangCodeAsync(VetFeedManagementContext context)
        {
            return GenerateUniqueCodeAsync(context, "LH", 6,
                async (ctx, code) => await ctx.LoHangs.AnyAsync(lh => lh.MaLoCode == code));
        }

        // Phiếu nhập 
        public static Task<string> GeneratePhieuNhapCodeAsync(VetFeedManagementContext context)
        {
            return GenerateUniqueCodeAsync(context, "PN", 8,
                async (ctx, code) => await ctx.PhieuNhaps.AnyAsync(pn => pn.MaPNCode == code));
        }

        // Phiếu bán
        public static Task<string> GeneratePhieuBanCodeAsync(VetFeedManagementContext context)
        {
            return GenerateUniqueCodeAsync(context, "PB", 8,
                async (ctx, code) => await ctx.PhieuBans.AnyAsync(pb => pb.MaPBCode == code));
        }

        // Phiếu chuyển kho
        public static Task<string> GeneratePhieuChuyenKhoCodeAsync(VetFeedManagementContext context)
        {
            return GenerateUniqueCodeAsync(context, "PCK", 8,
                async (ctx, code) => await ctx.PhieuChuyenKhos.AnyAsync(ck => ck.MaCKCode == code));
        }

        // Phiếu trả 
        public static Task<string> GeneratePhieuTraCodeAsync(VetFeedManagementContext context)
        {
            return GenerateUniqueCodeAsync(context, "PT", 8,
                async (ctx, code) => await ctx.PhieuTras.AnyAsync(pt => pt.MaPTCode == code));
        }
    }
}
