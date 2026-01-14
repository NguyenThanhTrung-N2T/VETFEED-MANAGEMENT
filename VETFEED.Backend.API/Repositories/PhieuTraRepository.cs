using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.PhieuTra;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Utils;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.DTOs.LoHang;

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
                // Load phiếu bán gốc + KH + CT + lô + sản phẩm
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

                //  Tạo phiếu trả
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

                // Duyệt từng lô trả
                foreach (var chiTietTraRequest in request.DanhSachChiTiet!)
                {
                    var maLo = chiTietTraRequest.MaLo;

                    // SỐ LƯỢNG ĐÃ LÀ ĐƠN VỊ CƠ SỞ
                    var soLuongTraCoSo = chiTietTraRequest.SoLuong;

                    if (soLuongTraCoSo <= 0)
                        throw new Exception("Số lượng trả không hợp lệ");

                    var ctBanCuaLo = phieuBanGoc.CTPhieuBans!
                        .Where(ct => ct.MaLo == maLo)
                        .ToList();

                    if (!ctBanCuaLo.Any())
                        throw new Exception($"Lô {maLo} không có trong phiếu bán gốc!");

                    var loHang = ctBanCuaLo.First().LoHang!;
                    var sanPham = loHang.SanPham!;
                    var donGiaBan = ctBanCuaLo.First().DonGia;

                    // Tổng số lượng đã bán (đơn vị cơ sở)
                    var tongSoLuongBanCoSo = ctBanCuaLo.Sum(ct => ct.SoLuongQuyDoi);

                    if (soLuongTraCoSo > tongSoLuongBanCoSo + 0.0001m)
                        throw new Exception($"Số lượng trả vượt quá số lượng đã bán của lô {maLo}!");

                    //  Phân bổ trả về các kho đã xuất
                    var soLuongConLai = soLuongTraCoSo;
                    var ghiChuPhanBo = new List<string>();

                    foreach (var ctBan in ctBanCuaLo.OrderBy(ct => ct.SoLuongQuyDoi))
                    {
                        if (soLuongConLai <= 0) break;

                        var tonKho = await _context.TonKhos
                            .Include(t => t.KhoHang)
                            .FirstOrDefaultAsync(t =>
                                t.MaKho == ctBan.MaKho &&
                                t.MaLo == maLo);

                        if (tonKho == null)
                            throw new Exception($"Không tìm thấy tồn kho kho {ctBan.MaKho} cho lô {maLo}");

                        var soLuongTraVaoKho = Math.Min(soLuongConLai, ctBan.SoLuongQuyDoi);

                        tonKho.SoLuongCoSo += soLuongTraVaoKho;
                        tonKho.NgayCapNhat = DateTime.Now;

                        ghiChuPhanBo.Add(
                            $"Kho {tonKho.KhoHang?.TenKho}: +{soLuongTraVaoKho} {sanPham.DonViCoSo}"
                        );

                        soLuongConLai -= soLuongTraVaoKho;
                    }

                    if (soLuongConLai > 0.0001m)
                        throw new Exception($"Không thể phân bổ trả hết cho lô {maLo}");

                    //  Tạo CT Phiếu Trả (1 dòng / lô)
                    var ctPhieuTra = new CTPhieuTra
                    {
                        MaCTPT = Guid.NewGuid(),
                        MaPT = phieuTra.MaPT,
                        MaLo = maLo,
                        SoLuongTra = soLuongTraCoSo, // đơn vị cơ sở
                        DonGiaHoan = donGiaBan,
                        DonViTra = sanPham.DonViCoSo,
                        GhiChu = $"{chiTietTraRequest.GhiChu} | Phân bổ: {string.Join("; ", ghiChuPhanBo)}"
                    };
                    _context.CTPhieuTras.Add(ctPhieuTra);

                    tongTienTra += soLuongTraCoSo * donGiaBan;
                }

                // Cập nhật tổng tiền phiếu trả
                phieuTra.ThanhTien = tongTienTra;

                // Xử lý công nợ + hoàn tiền
                if (tongTienTra > khachHang.CongNoHienTai)
                {
                    var tienTraNo = khachHang.CongNoHienTai;
                    var tienHoanThem = tongTienTra - tienTraNo;

                    if (tienTraNo > 0)
                    {
                        _context.CongNos.Add(new CongNo
                        {
                            MaCongNo = Guid.NewGuid(),
                            LoaiDoiTuong = LoaiDoiTuongCongNoEnum.KHACH_HANG,
                            MaDoiTuong = khachHang.MaKH,
                            MaPhieu = phieuTra.MaPT,
                            SoTien = -tienTraNo,
                            NgayPhatSinh = DateTime.Now,
                            GhiChu = $"GIAM NO: Phiếu trả {phieuTra.MaPTCode}"
                        });
                    }

                    if (tienHoanThem > 0)
                    {
                        _context.CongNos.Add(new CongNo
                        {
                            MaCongNo = Guid.NewGuid(),
                            LoaiDoiTuong = LoaiDoiTuongCongNoEnum.KHACH_HANG,
                            MaDoiTuong = khachHang.MaKH,
                            MaPhieu = phieuTra.MaPT,
                            SoTien = -tienHoanThem,
                            NgayPhatSinh = DateTime.Now,
                            GhiChu = $"HOAN TIEN: Phiếu trả {phieuTra.MaPTCode}"
                        });
                    }

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
                        GhiChu = $"GIAM NO: Phiếu trả {phieuTra.MaPTCode}"
                    });

                    khachHang.CongNoHienTai -= tongTienTra;
                    if (khachHang.CongNoHienTai < 0)
                        khachHang.CongNoHienTai = 0;
                }

                //  Rollback tổng mua
                khachHang.TongMua -= tongTienTra;
                if (khachHang.TongMua < 0)
                    khachHang.TongMua = 0;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetChiTietPhieuTraAsync(phieuTra.MaPT)
                    ?? throw new Exception("Không lấy được chi tiết phiếu trả");
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

                // Lấy toàn bộ CT phiếu bán gốc
                var ctPhieuBan = await _context.CTPhieuBans
                    .Where(x => x.MaPB == phieuTra.MaPB)
                    .ToListAsync();

                foreach (var ctTra in phieuTra.CTPhieuTras!)
                {
                    // SỐ LƯỢNG CẦN TRỪ (ĐƠN VỊ CƠ SỞ)
                    var soLuongCanTru = ctTra.SoLuongTra;

                    if (soLuongCanTru <= 0)
                        throw new Exception("Số lượng rollback không hợp lệ");

                    // Các CT bán của đúng lô (các kho đã xuất)
                    var ctBanTheoLo = ctPhieuBan
                        .Where(x => x.MaLo == ctTra.MaLo)
                        .ToList();

                    if (!ctBanTheoLo.Any())
                        throw new Exception("Không tìm thấy chi tiết phiếu bán cho lô trả");

                    var maKhoLienQuan = ctBanTheoLo.Select(x => x.MaKho).Distinct().ToList();

                    // Lấy tồn kho các kho liên quan
                    var tonKhos = await _context.TonKhos
                        .Where(t =>
                            t.MaLo == ctTra.MaLo &&
                            maKhoLienQuan.Contains(t.MaKho))
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
                        tk.NgayCapNhat = DateTime.Now;

                        soLuongCanTru -= tru;
                    }

                    if (soLuongCanTru > 0)
                        throw new Exception("Rollback tồn kho không đủ số lượng");
                }

                // Rollback công nợ
                var congNos = await _context.CongNos
                    .Where(cn => cn.MaPhieu == phieuTra.MaPT)
                    .ToListAsync();

                foreach (var cn in congNos)
                {
                    // Phiếu trả luôn tạo số âm → xóa thì cộng lại
                    khachHang.CongNoHienTai += Math.Abs(cn.SoTien);
                }

                // an toàn dữ liệu
                if (khachHang.CongNoHienTai < 0)
                    khachHang.CongNoHienTai = 0;

                // Rollback tổng mua
                khachHang.TongMua += tongTienTra;

                // Xóa dữ liệu
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

        /// <summary>
        /// Lấy số lượng có thể trả được cho một phiếu bán.
        /// Tính toán: SoLuongCoTheTra = SoLuongDaBan - SoLuongDaTra (từ các phiếu trả trước)
        /// </summary>
        public async Task<ReturnableQuantityResponse?> GetReturnableQuantityAsync(Guid maPB)
        {
            // 1. Lấy phiếu bán gốc kèm chi tiết
            var phieuBan = await _context.PhieuBans
                .Include(pb => pb.CTPhieuBans!)
                    .ThenInclude(ct => ct.LoHang)
                        .ThenInclude(lo => lo!.SanPham)
                .FirstOrDefaultAsync(pb => pb.MaPB == maPB);

            if (phieuBan == null)
                return null;

            // 2. Lấy tất cả phiếu trả liên quan đến phiếu bán này
            var phieuTras = await _context.PhieuTras
                .Include(pt => pt.CTPhieuTras)
                .Where(pt => pt.MaPB == maPB)
                .ToListAsync();

            // 3. Tính tổng số lượng đã trả theo từng lô
            var soLuongDaTraTheoLo = phieuTras
                .SelectMany(pt => pt.CTPhieuTras ?? new List<CTPhieuTra>())
                .GroupBy(ct => ct.MaLo)
                .ToDictionary(g => g.Key, g => g.Sum(ct => ct.SoLuongTra));

            // 4. Build response với thông tin từng lô
            var danhSachLoHang = phieuBan.CTPhieuBans!
                .GroupBy(ct => ct.MaLo)
                .Select(g =>
                {
                    var loHang = g.First().LoHang!;
                    var sanPham = loHang.SanPham!;
                    var soLuongDaBan = g.Sum(ct => ct.SoLuongQuyDoi);
                    var soLuongDaTra = soLuongDaTraTheoLo.GetValueOrDefault(g.Key, 0m);
                    var soLuongCoTheTra = soLuongDaBan - soLuongDaTra;

                    return new ReturnableLoHangItem
                    {
                        MaLo = g.Key,
                        MaLoCode = loHang.MaLoCode,
                        TenSanPham = sanPham.TenSP,
                        DonViCoSo = sanPham.DonViCoSo,
                        HanSuDung = loHang.HanSuDung,
                        SoLuongDaBan = soLuongDaBan,
                        SoLuongDaTra = soLuongDaTra,
                        SoLuongCoTheTra = soLuongCoTheTra > 0 ? soLuongCoTheTra : 0
                    };
                })
                .ToList();

            return new ReturnableQuantityResponse
            {
                MaPB = phieuBan.MaPB,
                MaPBCode = phieuBan.MaPBCode,
                DanhSachLoHang = danhSachLoHang
            };
        }



    }
}
