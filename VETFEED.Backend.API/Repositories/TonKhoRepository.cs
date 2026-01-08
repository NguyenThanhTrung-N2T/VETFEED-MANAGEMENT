using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.TonKho;
using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public class TonKhoRepository : ITonKhoRepository
    {
        private readonly VetFeedManagementContext _context;
        public TonKhoRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        // lay danh sach ton kho theo kho 
        public async Task<IEnumerable<TonKhoResponse>> GetListTonKhoByKhoAsync()
        {
            var result = await _context.KhoHangs
            .Select(kho => new TonKhoResponse
            {
                MaKho = kho.MaKho,
                TenKho = kho.TenKho!,
                DanhSachTonKho = _context.TonKhos
                    .Where(tk => tk.MaKho == kho.MaKho && tk.SoLuongCoSo > 0)
                    .Join(_context.LoHangs, tk => tk.MaLo, lo => lo.MaLo, (tk, lo) => new { tk, lo })
                    .Join(_context.SanPhams, x => x.lo.MaSP, sp => sp.MaSP, (x, sp) => new TonKhoItemResponse
                    {
                        MaLo = x.lo.MaLo,
                        TenSP = sp.TenSP!,
                        MaPNCode = _context.CTPhieuNhaps
                            .Where(ct => ct.MaLo == x.lo.MaLo)
                            .Select(ct => _context.PhieuNhaps
                                .Where(pn => pn.MaPN == ct.MaPN)
                                .Select(pn => pn.MaPNCode)
                                .FirstOrDefault())
                            .FirstOrDefault(),
                        DonGia = _context.CTPhieuNhaps
                            .Where(ct => ct.MaLo == x.lo.MaLo)
                            .Select(ct => ct.DonGia)
                            .FirstOrDefault(),
                        SoLuong = x.tk.SoLuongCoSo
                    })
                    .ToList()
            }).ToListAsync();

            // tra ve danh sach ton kho theo cac kho 
            return result;
        }

        // cap nhat so luong ton kho 
        public async Task<bool> UpdateTonKhoAsync(Guid maKho, Guid maLo, decimal soLuong) 
        { 
            // lay ton kho trong db
            var tonKho = await _context.TonKhos.FirstOrDefaultAsync(tk => tk.MaKho == maKho && tk.MaLo == maLo); 
            if (tonKho == null) 
                return false; 
            // cap nhat so luong
            tonKho.SoLuongCoSo = soLuong; 
            tonKho.NgayCapNhat = DateTime.UtcNow; 
            await _context.SaveChangesAsync(); 
            return true; 
        }

        // kiem tra ton kho 
        public async Task<bool> IsExistTonKho(Guid MaKho, Guid MaLo)
        {
            return await _context.TonKhos.AnyAsync(tk => tk.MaKho == MaKho && tk.MaLo == MaLo);
        }

        // kiem tra ton kho co du so luong hay khong 
        public async Task<bool> IsTonKhoEnough(Guid MaKho, Guid MaLo, decimal SoLuongChuyen)
        {
            // kiem tra ton kho 
            var tonKho = await _context.TonKhos.FirstOrDefaultAsync(tk => tk.MaKho == MaKho && tk.MaLo == MaLo);
            if (tonKho == null)
                return false;

            if (tonKho.SoLuongCoSo < SoLuongChuyen)
                return false;

            return true;
        }

        // lay ton kho theo ma kho va ma lo 
        public async Task<TonKhoChiTietResponse?> GetTonKhoAsync(Guid maKho, Guid maLo) 
        {
            // lay ton kho 
            var tonKho = await _context.TonKhos.FirstOrDefaultAsync(t => t.MaKho == maKho && t.MaLo == maLo); 
            if (tonKho == null) 
                return null; 
            return new TonKhoChiTietResponse 
            { 
                MaTK = tonKho.MaTonKho, 
                MaKho = tonKho.MaKho, 
                MaLo = tonKho.MaLo, 
                SoLuongTon = tonKho.SoLuongCoSo, 
                NgayCapNhat = tonKho.NgayCapNhat 
            }; 
        }

        // them ton kho 
        public async Task<TonKhoChiTietResponse> AddTonKhoAsync(Guid maKho, Guid maLo, decimal soLuong) 
        {
            // kiem tra neu da ton tai
            var existing = await _context.TonKhos.FirstOrDefaultAsync(tk => tk.MaKho == maKho && tk.MaLo == maLo);
            if (existing != null)
                return null; // Da ton tai, khong tao moi
            try
            {
                // tao ton kho
                var tonKho = new TonKho
                {
                    MaTonKho = Guid.NewGuid(),
                    MaKho = maKho,
                    MaLo = maLo,
                    SoLuongCoSo = soLuong,
                    NgayCapNhat = DateTime.Now
                };
                // them ton kho 
                _context.TonKhos.Add(tonKho);
                await _context.SaveChangesAsync();
                return new TonKhoChiTietResponse
                {
                    MaTK = tonKho.MaTonKho,
                    MaKho = tonKho.MaKho,
                    MaLo = tonKho.MaLo,
                    SoLuongTon = tonKho.SoLuongCoSo,
                    NgayCapNhat = tonKho.NgayCapNhat
                };
            } catch(Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi thêm tồn kho !", ex);
            }
            
        }

        // tang so luong ton kho 
        public async Task<bool> IncreaseTonKhoAsync(Guid MaKho, Guid MaLo, decimal soLuong)
        {
            // kiem tra ton kho 
            var tonKho = await _context.TonKhos.FirstOrDefaultAsync(tk => tk.MaKho == MaKho && tk.MaLo == MaLo);
            if(tonKho == null)
            {
                // them ton kho 
                tonKho = new TonKho
                {
                    MaTonKho = Guid.NewGuid(),
                    MaKho = MaKho,
                    MaLo = MaLo,
                    SoLuongCoSo = soLuong,
                    NgayCapNhat = DateTime.Now,
                };
                _context.TonKhos.Add(tonKho);
            }
            else
            {
                tonKho.SoLuongCoSo += soLuong;
                tonKho.NgayCapNhat = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // giam so luong ton kho 
        public async Task<bool> DecreaseTonKhoAsync(Guid MaKho, Guid MaLo, decimal soLuong)
        {
            // kiem tra ton kho 
            var tonKho = await _context.TonKhos.FirstOrDefaultAsync(tk => tk.MaKho == MaKho && tk.MaLo == MaLo);
            if (tonKho == null || tonKho.SoLuongCoSo < soLuong)
            {
                // khong du so luong 
                return false;
            }

            tonKho.SoLuongCoSo -= soLuong;
            tonKho.NgayCapNhat = DateTime.Now;

            // luu ton kho
            await _context.SaveChangesAsync();
            return true;

        }

        // kiem tra ton kho cua lo tai toan bo cac kho 
        public async Task<bool> IsTonKhoEnoughAllKhoAsync(Guid maLo, decimal soLuongCan)
        {
            // lay ton ton kho
            var tongTon = await _context.TonKhos.Where(t => t.MaLo == maLo).SumAsync(t => t.SoLuongCoSo); 
            return tongTon >= soLuongCan;
        }

        // tao moi ban ghi ton kho
        //public async Task<bool> AddTonKhoAsync(Guid maKho, Guid maLo, decimal soLuong)
        //{
        //    // kiem tra neu da ton tai
        //    var existing = await _context.TonKhos.FirstOrDefaultAsync(tk => tk.MaKho == maKho && tk.MaLo == maLo);
        //    if (existing != null)
        //        return false; // Da ton tai, khong tao moi

        //    var tonKho = new Models.TonKho
        //    {
        //        MaTonKho = Guid.NewGuid(),
        //        MaKho = maKho,
        //        MaLo = maLo,
        //        SoLuong = soLuong,
        //        NgayCapNhat = DateTime.UtcNow
        //    };

        //    _context.TonKhos.Add(tonKho);
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        // them hoac cap nhat ton kho (neu ton tai thi cong them so luong)
        public async Task<bool> AddOrUpdateTonKhoAsync(Guid maKho, Guid maLo, decimal soLuong)
        {
            var tonKho = await _context.TonKhos.FirstOrDefaultAsync(tk => tk.MaKho == maKho && tk.MaLo == maLo);
            
            if (tonKho != null)
            {
                // Da ton tai: cong them so luong
                tonKho.SoLuongCoSo += soLuong;
                tonKho.NgayCapNhat = DateTime.UtcNow;
            }
            else
            {
                // Chua ton tai: tao moi
                tonKho = new Models.TonKho
                {
                    MaTonKho = Guid.NewGuid(),
                    MaKho = maKho,
                    MaLo = maLo,
                    SoLuongCoSo = soLuong,
                    NgayCapNhat = DateTime.UtcNow
                };
                _context.TonKhos.Add(tonKho);
            }

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
