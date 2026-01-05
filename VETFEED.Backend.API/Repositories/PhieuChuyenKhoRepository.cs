using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.CTChuyenKho;
using VETFEED.Backend.API.DTOs.PhieuChuyenKho;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Utils;

namespace VETFEED.Backend.API.Repositories
{
    public class PhieuChuyenKhoRepository :IPhieuChuyenKhoRepository
    {
        private readonly VetFeedManagementContext _context;
        public PhieuChuyenKhoRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        // lay danh sach phieu chuyen kho 
        public async Task<IEnumerable<PhieuChuyenKhoResponse>> GetDanhSachPhieuChuyenKhoAsync() 
        {
            // lay danh sach phieu chuyen kho
            var result = await _context.PhieuChuyenKhos.Select(pck => new PhieuChuyenKhoResponse
            {
                MaCK = pck.MaCK,
                MaCKCode = pck.MaCKCode,
                NgayLap = pck.NgayLap,
                TenKhoXuat = _context.KhoHangs
                .Where(k => k.MaKho == pck.MaKhoXuat)
                .Select(k => k.TenKho).FirstOrDefault(),
                TenKhoNhan = _context.KhoHangs
                .Where(k => k.MaKho == pck.MaKhoNhan)
                .Select(k => k.TenKho).FirstOrDefault(),
                GhiChu = pck.GhiChu
            }).OrderByDescending(p => p.NgayLap).ToListAsync();            
            // tra ve danh sach phieu chuyen
            return result; 
        }

        // lay chi tiet phieu chuyen kho 
        public async Task<ChiTietPhieuChuyenKhoResponse?> GetChiTietPhieuChuyenKhoAsync(Guid maCK)
        {
            // lay phieu chuyen kho
            var phieuCK = await _context.PhieuChuyenKhos
                .Include(p => p.KhoXuat)
                .Include(p => p.KhoNhan).FirstOrDefaultAsync(p => p.MaCK == maCK);

            if(phieuCK == null)
            {
                return null;
            }

            // lay chi tiet phieu chuyen kho 
            var chitiet = await _context.CTPhieuChuyenKhos
                .Where(ct => ct.MaCK == maCK)
                .Include(ct => ct.LoHang)
                .ThenInclude(lh => lh!.SanPham)
                .Select(ct => new CTChuyenKhoResponse
                {
                    MaCTCK = ct.MaCTCK,
                    MaLo = ct.MaLo,
                    MaLoCode = ct.LoHang!.MaLoCode,
                    TenSanPham = ct.LoHang.SanPham!.TenSP,
                    LoaiSanPham = ct.LoHang.SanPham.LoaiSanPham.ToString(),
                    DonViTinh = ct.LoHang.SanPham.DonViTinh,
                    SoLuongChuyen = ct.SoLuongChuyen,
                    HanSuDung = ct.LoHang.HanSuDung,
                    GhiChu = ct.GhiChu,
                    TrangThai = ct.TrangThai.ToString(),
                    DonGia = _context.CTPhieuNhaps.Where(ctpn => ctpn.MaLo == ct.MaLo).Select(ctpn => ctpn.DonGia).FirstOrDefault()
                }).ToListAsync();

            return new ChiTietPhieuChuyenKhoResponse 
            { 
                MaCK = phieuCK.MaCK, 
                MaCKCode = phieuCK.MaCKCode, 
                NgayLap = phieuCK.NgayLap, 
                TenKhoXuat = phieuCK.KhoXuat!.TenKho, 
                TenKhoNhan = phieuCK.KhoNhan!.TenKho, 
                GhiChu = phieuCK.GhiChu, 
                MaKhoXuat = phieuCK.MaKhoXuat, 
                MaKhoNhan = phieuCK.MaKhoNhan, 
                DanhSachSanPham = chitiet 
            };
        }

        // them phieu chuyen kho 
        public async Task<ChiTietPhieuChuyenKhoResponse> AddPhieuChuyenKhoAsync(PhieuChuyenKhoRequest request)
        {
            try
            {
                // tao phieu chuyen kho
                var phieu = new PhieuChuyenKho
                {
                    MaCKCode = await CodeGenerator.GeneratePhieuChuyenKhoCodeAsync(_context),
                    NgayLap = request.NgayLap,
                    MaKhoXuat = request.MaKhoXuat,
                    MaKhoNhan = request.MaKhoNhan,
                    GhiChu = request.GhiChu
                };
                // them phieu 
                _context.PhieuChuyenKhos.Add(phieu);

                // danh sach chi tiet chuyen kho 
                var chiTietList = new List<CTChuyenKhoResponse>();

                // them cac chi tiet chuyen kho
                foreach (var item in request.DanhSachSanPham!)
                {
                    var ct = new CTPhieuChuyenKho
                    {
                        MaCTCK = Guid.NewGuid(),
                        MaCK = phieu.MaCK,
                        MaLo = item.MaLo,
                        SoLuongChuyen = item.SoLuongChuyen,
                        TrangThai = item.TrangThai,
                        GhiChu = item.GhiChu
                    };
                    // them chi tiet chuyen kho
                    _context.CTPhieuChuyenKhos.Add(ct);

                    var lo = await _context.LoHangs
                        .Include(l => l.SanPham)
                        .FirstOrDefaultAsync(l => l.MaLo == item.MaLo);

                    var donGia = await _context.CTPhieuNhaps
                        .Where(ctpn => ctpn.MaLo == item.MaLo)
                        .Select(ctpn => ctpn.DonGia)
                        .FirstOrDefaultAsync();

                    // tra ve danh sach chi tiet chuyen kho
                    chiTietList.Add(new CTChuyenKhoResponse
                    {
                        MaCTCK = ct.MaCTCK,
                        MaLo = lo!.MaLo,
                        MaLoCode = lo!.MaLoCode,
                        TenSanPham = lo.SanPham!.TenSP,
                        LoaiSanPham = lo.SanPham.LoaiSanPham.ToString(),
                        DonViTinh = lo.SanPham.DonViTinh,
                        SoLuongChuyen = item.SoLuongChuyen,
                        HanSuDung = lo.HanSuDung,
                        GhiChu = item.GhiChu,
                        TrangThai = item.TrangThai.ToString(),
                        DonGia = donGia
                    });
                }

                // luu toan bo phieu va chi tiet chuyen kho
                await _context.SaveChangesAsync();

                // lay thong tin kho xuat va kho nhan
                var khoXuat = await _context.KhoHangs.FindAsync(request.MaKhoXuat);
                var khoNhan = await _context.KhoHangs.FindAsync(request.MaKhoNhan);

                // tra ve chi tiet phieu chuyen kho vua duoc them
                return new ChiTietPhieuChuyenKhoResponse
                {
                    MaCK = phieu.MaCK,
                    MaCKCode = phieu.MaCKCode,
                    NgayLap = request.NgayLap,
                    MaKhoXuat = request.MaKhoXuat,
                    MaKhoNhan = request.MaKhoNhan,
                    TenKhoXuat = khoXuat?.TenKho,
                    TenKhoNhan = khoNhan?.TenKho,
                    GhiChu = request.GhiChu,
                    DanhSachSanPham = chiTietList
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi tạo phiếu chuyển kho !", ex);
            }

        }

        // cap nhat chi tiet phieu chuyen kho
        public async Task<ChiTietPhieuChuyenKhoResponse?> UpdatePhieuChuyenKhoAsync(UpdatePhieuChuyenKhoRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // lay phieu chuyen kho trong db
                var phieu = await _context.PhieuChuyenKhos.FindAsync(request.MaCK);
                if (phieu == null) return null;

                // KHÔNG cho phép đổi kho xuất
                phieu.NgayLap = request.NgayLap;
                phieu.MaKhoNhan = request.MaKhoNhan;
                phieu.GhiChu = request.GhiChu;

                // xử lý chi tiết
                foreach (var item in request.DanhSachChiTiet!)
                {
                    if (item.IsDeleted && item.MaCTCK.HasValue)
                    {
                        // xóa chi tiết
                        var ct = await _context.CTPhieuChuyenKhos.FindAsync(item.MaCTCK.Value);
                        if (ct != null) _context.CTPhieuChuyenKhos.Remove(ct);
                    }
                    else if (!item.MaCTCK.HasValue)
                    {
                        // thêm mới
                        var ct = new CTPhieuChuyenKho
                        {
                            MaCTCK = Guid.NewGuid(),
                            MaCK = request.MaCK,
                            MaLo = item.MaLo,
                            SoLuongChuyen = item.SoLuongChuyen,
                            GhiChu = item.GhiChu,
                            TrangThai = item.TrangThai
                        };
                        _context.CTPhieuChuyenKhos.Add(ct);
                    }
                    else
                    {
                        // cập nhật chi tiết
                        var ct = await _context.CTPhieuChuyenKhos.FindAsync(item.MaCTCK.Value);
                        if (ct != null)
                        {
                            ct.MaLo = item.MaLo;
                            ct.SoLuongChuyen = item.SoLuongChuyen;
                            ct.GhiChu = item.GhiChu;
                            ct.TrangThai = item.TrangThai;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return await GetChiTietPhieuChuyenKhoAsync(request.MaCK);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception("Lỗi khi cập nhật phiếu chuyển kho!");
            }
        }


    }
}
