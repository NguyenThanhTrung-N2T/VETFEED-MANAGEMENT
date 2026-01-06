using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.TonKho;

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
                    .Where(tk => tk.MaKho == kho.MaKho && tk.SoLuong > 0)
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
                        SoLuong = x.tk.SoLuong
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
            tonKho.SoLuong = soLuong; 
            tonKho.NgayCapNhat = DateTime.UtcNow; 
            await _context.SaveChangesAsync(); 
            return true; 
        }

        // tao moi ban ghi ton kho
        public async Task<bool> AddTonKhoAsync(Guid maKho, Guid maLo, decimal soLuong)
        {
            // kiem tra neu da ton tai
            var existing = await _context.TonKhos.FirstOrDefaultAsync(tk => tk.MaKho == maKho && tk.MaLo == maLo);
            if (existing != null)
                return false; // Da ton tai, khong tao moi

            var tonKho = new Models.TonKho
            {
                MaTonKho = Guid.NewGuid(),
                MaKho = maKho,
                MaLo = maLo,
                SoLuong = soLuong,
                NgayCapNhat = DateTime.UtcNow
            };

            _context.TonKhos.Add(tonKho);
            await _context.SaveChangesAsync();
            return true;
        }

        // them hoac cap nhat ton kho (neu ton tai thi cong them so luong)
        public async Task<bool> AddOrUpdateTonKhoAsync(Guid maKho, Guid maLo, decimal soLuong)
        {
            var tonKho = await _context.TonKhos.FirstOrDefaultAsync(tk => tk.MaKho == maKho && tk.MaLo == maLo);
            
            if (tonKho != null)
            {
                // Da ton tai: cong them so luong
                tonKho.SoLuong += soLuong;
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
                    SoLuong = soLuong,
                    NgayCapNhat = DateTime.UtcNow
                };
                _context.TonKhos.Add(tonKho);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
