using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.PhieuTra;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Utils;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.Repositories
{
    public class PhieuTraRepository : IPhieuTraRepository
    {
        private readonly VetFeedManagementContext _context;

        public PhieuTraRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        // Lấy danh sách phiếu trả
        public async Task<IEnumerable<PhieuTraListResponse>> GetDanhSachPhieuTraAsync()
        {
            var result = await _context.PhieuTras
                .Include(pt => pt.PhieuBan)
                .Include(pt => pt.KhachHang)
                .Select(pt => new PhieuTraListResponse
                {
                    MaPT = pt.MaPT,
                    MaPTCode = pt.MaPTCode ?? "",
                    MaPB = pt.MaPB,
                    MaPBCode = pt.PhieuBan!.MaPBCode,
                    NgayTra = pt.NgayTra,
                    MaKH = pt.MaKH,
                    TenKhachHang = pt.KhachHang!.TenKH,
                    ThanhTien = pt.ThanhTien,
                    HinhThucHoanTien = pt.HinhThucHoanTien.ToString()
                })
                .OrderByDescending(pt => pt.NgayTra)
                .ToListAsync();

            return result;
        }


        //Lấy chi tiết phiếu trả
        public async Task<PhieuTraDetailResponse?> GetChiTietPhieuTraAsync(Guid maPT)
        {
            var phieuTra = await _context.PhieuTras
                .Include(pt => pt.PhieuBan!)
                    .ThenInclude(pb => pb!.CTPhieuBans!)
                        .ThenInclude(ct => ct.LoHang)
                            .ThenInclude(lo => lo!.SanPham)
                .Include(pt => pt.KhachHang)
                .Include(pt => pt.CTPhieuTras!)
                    .ThenInclude(ct => ct.LoHang)
                        .ThenInclude(lo => lo!.SanPham)
                .FirstOrDefaultAsync(pt => pt.MaPT == maPT);

            if (phieuTra == null)
                return null;

            var phieuBanGoc = phieuTra.PhieuBan!;
            var ctPhieuTras = phieuTra.CTPhieuTras!;

            var chiTietList = ctPhieuTras
                .GroupBy(ct => ct.MaLo)
                .Select(group =>
                {
                    var dauTien = group.First();
                    var loHang = dauTien.LoHang!;
                    var sanPham = loHang.SanPham!;

                    // Lấy DonViBan từ phiếu bán gốc (tất cả chi tiết cùng lô có cùng DonViBan)
                    var donViBanTuPhieuBanGoc = phieuBanGoc.CTPhieuBans!
                        .FirstOrDefault(ct => ct.MaLo == dauTien.MaLo)?
                        .DonViBan ?? sanPham.DonViCoSo;

                    return new ChiTietPhieuTraDetailResponse
                    {
                        MaCTPT = dauTien.MaCTPT,
                        MaLo = dauTien.MaLo,
                        MaLoCode = loHang.MaLoCode,
                        TenSanPham = sanPham.TenSP,
                        DonViCoSo = sanPham.DonViCoSo,
                        DonViTra = donViBanTuPhieuBanGoc,
                        SoLuongTra = group.Sum(ct => ct.SoLuongTra),
                        DonGiaHoan = dauTien.DonGiaHoan,
                        ThanhTienTra = group.Sum(ct => ct.SoLuongTra) * dauTien.DonGiaHoan,
                        HanSuDung = loHang.HanSuDung,
                        GhiChu = string.Join(" | ", group.Select(ct => ct.GhiChu).Where(x => !string.IsNullOrEmpty(x)))
                    };
                })
                .ToList();

            return new PhieuTraDetailResponse
            {
                MaPT = phieuTra.MaPT,
                MaPTCode = phieuTra.MaPTCode ?? "",
                MaPB = phieuTra.MaPB,
                MaPBCode = phieuBanGoc.MaPBCode,
                MaKH = phieuTra.MaKH,
                TenKhachHang = phieuTra.KhachHang!.TenKH,
                NgayTra = phieuTra.NgayTra,
                ThanhTien = phieuTra.ThanhTien,
                LyDoTra = phieuTra.LyDo,
                HinhThucHoanTien = phieuTra.HinhThucHoanTien.ToString(),
                DanhSachChiTiet = chiTietList
            };
        }

        // Tạo phiếu trả hàng mới
        public async Task<PhieuTraDetailResponse> CreatePhieuTraAsync(CreatePhieuTraRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Load phiếu bán gốc + KH + chi tiết + lô + sản phẩm
                var phieuBanGoc = await _context.PhieuBans
                    .Include(pb => pb.KhachHang!)
                    .Include(pb => pb.CTPhieuBans!)
                        .ThenInclude(ct => ct.LoHang)
                            .ThenInclude(lo => lo!.SanPham)
                    .FirstOrDefaultAsync(pb => pb.MaPB == request.MaPB);

                if (phieuBanGoc == null)
                    throw new Exception("Phiếu bán không tồn tại!");

                var khachHang = phieuBanGoc.KhachHang!;
                decimal tongTienTra = 0m;

                // Tạo phiếu trả 
                var phieuTra = new PhieuTra
                {
                    MaPTCode = await CodeGenerator.GeneratePhieuTraCodeAsync(_context),
                    MaPB = request.MaPB,
                    MaKH = phieuBanGoc.MaKH,
                    NgayTra = DateTime.Now,
                    ThanhTien = 0m,
                    LyDo = request.LyDoTra,
                    HinhThucHoanTien = request.HinhThucHoanTien
                };
                _context.PhieuTras.Add(phieuTra);
                await _context.SaveChangesAsync();

                // Duyệt từng lô trong request (1 CTPhieuTra cho mỗi lô)
                foreach (var chiTietTraRequest in request.DanhSachChiTiet!)
                {
                    var maLo = chiTietTraRequest.MaLo;
                    var soLuongTraTheoDonViBan = chiTietTraRequest.SoLuong;
                    var donViTra = chiTietTraRequest.DonViTra;

                    var ctBanCuaLo = phieuBanGoc.CTPhieuBans!
                        .Where(ct => ct.MaLo == maLo)
                        .ToList();

                    if (!ctBanCuaLo.Any())
                        throw new Exception($"Lô {maLo} không có trong phiếu bán gốc!");

                    var loHang = ctBanCuaLo.First().LoHang!;
                    var sanPham = loHang.SanPham!;
                    var donGiaBan = ctBanCuaLo.First().DonGia;

                    // Quy đổi đơn vị trả → đơn vị cơ sở
                    decimal tyLeQuyDoi = 1m;
                    if (!string.Equals(donViTra, sanPham.DonViCoSo, StringComparison.OrdinalIgnoreCase))
                    {
                        var qd = await _context.QuyDoiDonVis
                            .FirstOrDefaultAsync(x => x.MaSP == sanPham.MaSP && x.DonViNhap == donViTra);

                        if (qd == null)
                            throw new Exception($"Không tìm thấy quy đổi từ {donViTra} sang {sanPham.DonViCoSo} cho sản phẩm {sanPham.TenSP}!");

                        tyLeQuyDoi = qd.TyLe;
                    }

                    var soLuongTraCoSo = soLuongTraTheoDonViBan * tyLeQuyDoi;
                    var tongSoLuongBanCoSo = ctBanCuaLo.Sum(ct => ct.SoLuongQuyDoi);

                    if (soLuongTraCoSo > tongSoLuongBanCoSo + 0.0001m)
                        throw new Exception($"Số lượng trả vượt quá số lượng đã bán của lô {maLo}!");

                    // Phân bổ tồn kho theo các kho đã xuất
                    var soLuongConLai = soLuongTraCoSo;
                    var ghiChuPhanBo = new List<string>();

                    foreach (var ctBan in ctBanCuaLo.OrderBy(ct => ct.SoLuongQuyDoi))
                    {
                        if (soLuongConLai <= 0) break;

                        var tk = await _context.TonKhos
                            .Include(t => t.KhoHang)
                            .FirstOrDefaultAsync(t => t.MaKho == ctBan.MaKho && t.MaLo == maLo);

                        if (tk == null)
                            throw new Exception($"Không tìm thấy tồn kho cho kho {ctBan.MaKho} và lô {maLo}!");

                        var soLuongTraVaoKhoNayCoSo = Math.Min(soLuongConLai, ctBan.SoLuongQuyDoi);
                        tk.SoLuongCoSo += soLuongTraVaoKhoNayCoSo;

                        var soLuongTraTheoDonVi = soLuongTraVaoKhoNayCoSo / tyLeQuyDoi;
                        ghiChuPhanBo.Add($"Kho {tk.KhoHang?.TenKho}: +{soLuongTraVaoKhoNayCoSo} {sanPham.DonViCoSo} (~{soLuongTraTheoDonVi} {donViTra})");

                        soLuongConLai -= soLuongTraVaoKhoNayCoSo;
                    }

                    if (soLuongConLai > 0.0001m)
                        throw new Exception($"Không thể phân bổ trả hết cho lô {maLo}. Còn thiếu {soLuongConLai} {sanPham.DonViCoSo}.");

                    // Tạo 1 CTPhieuTra duy nhất cho lô
                    var ctPhieuTra = new CTPhieuTra
                    {
                        MaCTPT = Guid.NewGuid(),
                        MaPT = phieuTra.MaPT,
                        MaLo = maLo,
                        SoLuongTra = soLuongTraTheoDonViBan,
                        DonGiaHoan = donGiaBan,
                        DonViTra = donViTra,
                        GhiChu = $"{chiTietTraRequest.GhiChu} | Phân bổ: {string.Join("; ", ghiChuPhanBo)}"
                    };
                    _context.CTPhieuTras.Add(ctPhieuTra);

                    tongTienTra += (soLuongTraTheoDonViBan * donGiaBan);
                }

                // Cập nhật tổng tiền phiếu trả
                phieuTra.ThanhTien = tongTienTra;

                // Công nợ
                if (tongTienTra > khachHang.CongNoHienTai)
                {
                    var tienTraNo = khachHang.CongNoHienTai;
                    var tienHoanThem = tongTienTra - tienTraNo;

                    _context.CongNos.Add(new CongNo
                    {
                        MaCongNo = Guid.NewGuid(),
                        LoaiDoiTuong = LoaiDoiTuongCongNoEnum.KHACH_HANG,
                        MaDoiTuong = khachHang.MaKH,
                        MaPhieu = phieuTra.MaPT,
                        SoTien = -tienTraNo,
                        NgayPhatSinh = DateTime.Now,
                        GhiChu = $"GIAM TIEN: Trả nợ từ phiếu trả {phieuTra.MaPTCode}"
                    });

                    _context.CongNos.Add(new CongNo
                    {
                        MaCongNo = Guid.NewGuid(),
                        LoaiDoiTuong = LoaiDoiTuongCongNoEnum.KHACH_HANG,
                        MaDoiTuong = khachHang.MaKH,
                        MaPhieu = phieuTra.MaPT,
                        SoTien = -tienHoanThem,
                        NgayPhatSinh = DateTime.Now,
                        GhiChu = $"HOAN TIEN: Hoàn tiền thừa từ phiếu trả {phieuTra.MaPTCode}"
                    });

                    khachHang.CongNoHienTai = 0;
                }
                else
                {
                    _context.CongNos.Add(new CongNo
                    {
                        MaCongNo = Guid.NewGuid(),
                        LoaiDoiTuong = LoaiDoiTuongCongNoEnum.KHACH_HANG,
                        MaDoiTuong = khachHang.MaKH,
                        MaPhieu = phieuTra.MaPT,
                        SoTien = -tongTienTra,
                        NgayPhatSinh = DateTime.Now,
                        GhiChu = $"GIAM TIEN: Giảm nợ từ phiếu trả {phieuTra.MaPTCode}"
                    });

                    khachHang.CongNoHienTai -= tongTienTra;
                    if (khachHang.CongNoHienTai < 0) khachHang.CongNoHienTai = 0;
                }

                // Cập nhật tổng mua
                khachHang.TongMua -= tongTienTra;
                if (khachHang.TongMua < 0) khachHang.TongMua = 0;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Trả về chi tiết phiếu trả vừa tạo
                return await GetChiTietPhieuTraAsync(phieuTra.MaPT) ?? throw new Exception("Lỗi khi lấy chi tiết phiếu trả vừa tạo!");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        //Xóa phiếu trả
        public async Task<bool> DeletePhieuTraAsync(Guid maPT)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var phieuTra = await _context.PhieuTras
                    .Include(pt => pt.KhachHang)
                    .Include(pt => pt.CTPhieuTras)
                    .FirstOrDefaultAsync(pt => pt.MaPT == maPT);

                if (phieuTra == null)
                    return false;

                var khachHang = phieuTra.KhachHang!;
                var tongTienTra = phieuTra.ThanhTien;

                // Lấy toàn bộ CT phiếu bán gốc (để biết các kho liên quan)
                var ctPhieuBan = await _context.CTPhieuBans
                    .Include(x => x.LoHang)
                        .ThenInclude(lo => lo!.SanPham)
                    .Where(x => x.MaPB == phieuTra.MaPB)
                    .ToListAsync();

                foreach (var ctTra in phieuTra.CTPhieuTras!)
                {
                    // Các CT bán của đúng lô
                    var ctBanTheoLo = ctPhieuBan
                        .Where(x => x.MaLo == ctTra.MaLo)
                        .ToList();

                    if (!ctBanTheoLo.Any())
                        throw new Exception("Không tìm thấy chi tiết phiếu bán cho lô trả");

                    var sanPham = ctBanTheoLo.First().LoHang!.SanPham!;
                    decimal tyLe = 1m;

                    if (!string.Equals(
                        ctTra.DonViTra!.Trim(),
                        sanPham.DonViCoSo!.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        var qd = await _context.QuyDoiDonVis.FirstOrDefaultAsync(x =>
                            x.MaSP == sanPham.MaSP &&
                            x.DonViNhap == ctTra.DonViTra);

                        if (qd == null)
                            throw new Exception($"Không tìm thấy quy đổi cho đơn vị {ctTra.DonViTra}");

                        tyLe = qd.TyLe; // ví dụ: 1 hộp = 10 viên
                    }

                    var soLuongCanTru = ctTra.SoLuongTra * tyLe;

                    if (soLuongCanTru <= 0)
                        throw new Exception("Số lượng rollback không hợp lệ");

                    var tonKhos = await _context.TonKhos
                        .Where(t =>
                            t.MaLo == ctTra.MaLo &&
                            ctBanTheoLo.Select(x => x.MaKho).Contains(t.MaKho))
                        .OrderBy(t => t.SoLuongCoSo) // trừ kho nhỏ trước
                        .ToListAsync();

                    foreach (var tk in tonKhos)
                    {
                        if (soLuongCanTru <= 0)
                            break;

                        if (tk.SoLuongCoSo <= 0)
                            continue;

                        var tru = Math.Min(tk.SoLuongCoSo, soLuongCanTru);

                        tk.SoLuongCoSo -= tru;
                        tk.NgayCapNhat = DateTime.UtcNow;

                        soLuongCanTru -= tru;
                    }

                    if (soLuongCanTru > 0)
                        throw new Exception("Rollback tồn kho không đủ số lượng");
                }

                var congNos = await _context.CongNos
                    .Where(cn => cn.MaPhieu == phieuTra.MaPT)
                    .ToListAsync();

                foreach (var cn in congNos)
                {
                    if (cn.SoTien < 0)
                        khachHang.CongNoHienTai += Math.Abs(cn.SoTien);
                    else
                        khachHang.CongNoHienTai -= cn.SoTien;
                }

                if (khachHang.CongNoHienTai < 0)
                    khachHang.CongNoHienTai = 0;

                khachHang.TongMua += tongTienTra;

                _context.CongNos.RemoveRange(congNos);
                _context.CTPhieuTras.RemoveRange(phieuTra.CTPhieuTras);
                _context.PhieuTras.Remove(phieuTra);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }


    }
}
